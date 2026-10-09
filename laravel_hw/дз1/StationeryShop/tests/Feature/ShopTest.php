<?php

namespace Tests\Feature;

use App\Models\Order;
use App\Models\Product;
use App\Models\User;
use App\Services\OrderService;
use Illuminate\Foundation\Testing\RefreshDatabase;
use Illuminate\Validation\ValidationException;
use Tests\TestCase;

class ShopTest extends TestCase
{
    use RefreshDatabase;

    protected function setUp(): void
    {
        parent::setUp();
        $this->seed();
    }

    private function customer(): array
    {
        return ['customer_name' => 'Тестовый покупатель', 'email' => 'buyer@example.com', 'phone' => '+380501234567', 'address' => 'Киев, улица Примерная, 1'];
    }

    public function test_catalog_search_and_category(): void
    {
        $this->get('/')->assertOk()->assertSee('КанцМаркет')->assertSee('Тетрадь 48 листов');
        $this->get('/?search=Карандаш')->assertOk()->assertSee('Карандаш HB')->assertDontSee('Тетрадь 48 листов');
        $this->get('/?category='.urlencode('Папки'))->assertOk()->assertSee('Папка А4')->assertDontSee('Карандаш HB');
        $this->get('/products/'.Product::first()->id)->assertOk();
    }

    public function test_cart_add_update_remove_and_stock_validation(): void
    {
        $product = Product::first();
        $this->post('/cart/'.$product->id, ['quantity' => 2])->assertSessionHas('cart.'.$product->id, 2);
        $this->patch('/cart/'.$product->id, ['quantity' => 3])->assertSessionHas('cart.'.$product->id, 3);
        $this->get('/cart')->assertOk()->assertSee($product->name);
        $this->post('/cart/'.$product->id, ['quantity' => 999])->assertSessionHasErrors('cart');
        $this->delete('/cart/'.$product->id)->assertSessionMissing('cart.'.$product->id);
    }

    public function test_checkout_creates_order_decrements_stock_and_keeps_prices(): void
    {
        $product = Product::first();
        $stock = $product->stock;
        $price = $product->price_cents;
        $this->withSession(['cart' => [$product->id => 2]])->get('/checkout')->assertOk();
        $response = $this->post('/checkout', $this->customer());
        $order = Order::with('items')->firstOrFail();
        $response->assertRedirect(route('orders.show', $order->public_token))->assertSessionMissing('cart');
        $this->assertSame($stock - 2, $product->fresh()->stock);
        $this->assertEquals($price * 2, $order->total_cents);
        $product->update(['price_cents' => $price + 1000, 'name' => 'Другое имя']);
        $this->assertEquals($price, $order->items->first()->price_cents);
        $this->get('/orders/'.$order->public_token)->assertOk()->assertSee('Заказ №'.$order->id);
        $this->post('/checkout', $this->customer())->assertSessionHasErrors('cart');
        $this->assertDatabaseCount('orders', 1);
    }

    public function test_order_transaction_rolls_back_every_stock_change(): void
    {
        $first = Product::first();
        $second = Product::skip(1)->first();
        $stock = $first->stock;
        try {
            app(OrderService::class)->place($this->customer(), [$first->id => 1, $second->id => $second->stock + 1]);
            $this->fail('Заказ не должен быть создан.');
        } catch (ValidationException $exception) {
            $this->assertArrayHasKey('cart', $exception->errors());
        }
        $this->assertSame($stock, $first->fresh()->stock);
        $this->assertDatabaseCount('orders', 0);
        $this->assertDatabaseCount('order_items', 0);
    }

    public function test_admin_login_protection_and_logout(): void
    {
        $this->get('/admin')->assertRedirect(route('admin.login'));
        $this->get('/admin/products')->assertRedirect(route('admin.login'));
        $this->get('/admin/login')->assertOk();
        $this->post('/admin/login', ['email' => 'admin@stationery.local', 'password' => 'incorrect'])->assertSessionHasErrors('email');
        $this->post('/admin/login', ['email' => 'admin@stationery.local', 'password' => 'Admin12345!'])->assertRedirect(route('admin.dashboard'));
        $this->get('/admin')->assertOk();
        $this->post('/admin/logout')->assertRedirect(route('catalog.index'));
        $this->get('/admin')->assertRedirect(route('admin.login'));
    }

    public function test_admin_product_crud_and_order_status(): void
    {
        $this->actingAs(User::first());
        $this->get('/admin/products/create')->assertOk();
        $data = ['name' => 'Линейка', 'category' => 'Аксессуары', 'description' => 'Линейка 20 см', 'price' => '25.50', 'stock' => 10];
        $this->post('/admin/products', $data)->assertRedirect(route('admin.products.index'));
        $product = Product::where('name', 'Линейка')->firstOrFail();
        $this->assertSame(2550, $product->price_cents);
        $this->get('/admin/products/'.$product->id.'/edit')->assertOk();
        $this->put('/admin/products/'.$product->id, array_replace($data, ['stock' => 12]))->assertRedirect(route('admin.products.index'));
        $order = app(OrderService::class)->place($this->customer(), [$product->id => 1]);
        $this->get('/admin/orders')->assertOk();
        $this->get('/admin/orders/'.$order->id)->assertOk();
        $this->patch('/admin/orders/'.$order->id, ['status' => 'processing'])->assertSessionHasNoErrors();
        $this->assertSame('processing', $order->fresh()->status);
        $this->delete('/admin/products/'.$product->id)->assertRedirect(route('admin.products.index'));
        $this->assertSoftDeleted('products', ['id' => $product->id]);
        $this->get('/orders/'.$order->public_token)->assertOk()->assertSee('Линейка');
    }

    public function test_validation_and_non_admin_access(): void
    {
        $user = User::create(['name' => 'Покупатель', 'email' => 'customer@example.com', 'password' => 'Password123!', 'is_admin' => false]);
        $this->actingAs($user)->get('/admin')->assertForbidden();
        $this->actingAs(User::where('is_admin', true)->first());
        $this->post('/admin/products', ['name' => '', 'price' => -1, 'stock' => -1])->assertSessionHasErrors(['name', 'category', 'description', 'price', 'stock']);
        $this->withSession(['cart' => [Product::first()->id => 1]])->post('/checkout', ['email' => 'invalid'])->assertSessionHasErrors(['customer_name', 'email', 'phone', 'address']);
    }

    public function test_html_is_escaped(): void
    {
        $product = Product::first();
        $product->update(['name' => '<script>alert(1)</script>']);
        $this->get('/products/'.$product->id)->assertOk()->assertDontSee('<script>alert(1)</script>', false)->assertSee('&lt;script&gt;alert(1)&lt;/script&gt;', false);
    }
}

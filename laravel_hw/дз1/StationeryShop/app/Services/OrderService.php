<?php

namespace App\Services;

use App\Models\Order;
use App\Models\Product;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Str;
use Illuminate\Validation\ValidationException;

class OrderService
{
    public function place(array $customer, array $cart): Order
    {
        if ($cart === []) {
            throw ValidationException::withMessages(['cart' => 'Корзина пуста.']);
        }

        ksort($cart);

        return DB::transaction(function () use ($customer, $cart) {
            $items = [];
            $total = 0;

            foreach ($cart as $id => $quantity) {
                $product = Product::whereKey($id)->lockForUpdate()->first();
                if (! $product || ! is_int($quantity) || $quantity < 1 || $quantity > 1000 || $quantity > $product->stock) {
                    throw ValidationException::withMessages(['cart' => 'Один из товаров недоступен в нужном количестве. Проверьте корзину.']);
                }

                $updated = Product::whereKey($id)->where('stock', '>=', $quantity)->decrement('stock', $quantity);
                if ($updated !== 1) {
                    throw ValidationException::withMessages(['cart' => 'Остаток товара изменился. Проверьте корзину.']);
                }

                $items[] = ['product_id' => $product->id, 'name' => $product->name, 'price_cents' => $product->price_cents, 'quantity' => $quantity];
                $total += $product->price_cents * $quantity;
            }

            $order = Order::create($customer + ['total_cents' => $total, 'status' => 'new', 'public_token' => (string) Str::uuid()]);
            $order->items()->createMany($items);

            return $order;
        }, 3);
    }
}

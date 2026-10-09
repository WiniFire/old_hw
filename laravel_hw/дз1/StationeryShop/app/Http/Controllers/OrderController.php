<?php

namespace App\Http\Controllers;

use App\Models\Order;
use App\Services\CartService;
use App\Services\OrderService;
use Illuminate\Http\Request;

class OrderController
{
    public function create(CartService $cart)
    {
        $items = $cart->items();
        if ($items->isEmpty()) return redirect()->route('cart.index')->with('error', 'Добавьте товары в корзину.');
        $total = $items->sum('subtotal');

        return view('orders.create', compact('items', 'total'));
    }

    public function store(Request $request, CartService $cart, OrderService $orders)
    {
        $data = $request->validate([
            'customer_name' => 'required|string|max:100',
            'email' => 'required|email|max:255',
            'phone' => ['required', 'string', 'max:30', 'regex:/^[+0-9()\s-]{7,30}$/'],
            'address' => 'required|string|max:300',
        ]);

        $order = $orders->place($data, $cart->quantities());
        $cart->clear();

        return redirect()->route('orders.show', $order->public_token)->with('success', 'Заказ успешно оформлен.');
    }

    public function show(string $token)
    {
        $order = Order::with('items')->where('public_token', $token)->firstOrFail();

        return view('orders.show', compact('order'));
    }
}

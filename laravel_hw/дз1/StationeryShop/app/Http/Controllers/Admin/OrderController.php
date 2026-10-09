<?php

namespace App\Http\Controllers\Admin;

use App\Models\Order;
use Illuminate\Http\Request;
use Illuminate\Validation\Rule;

class OrderController
{
    public function index()
    {
        $orders = Order::latest()->paginate(15);

        return view('admin.orders.index', compact('orders'));
    }

    public function show(Order $order)
    {
        $order->load('items');

        return view('admin.orders.show', compact('order'));
    }

    public function update(Request $request, Order $order)
    {
        $data = $request->validate(['status' => ['required', Rule::in(['new', 'processing', 'completed'])]]);
        $order->update($data);

        return back()->with('success', 'Статус заказа обновлён.');
    }
}

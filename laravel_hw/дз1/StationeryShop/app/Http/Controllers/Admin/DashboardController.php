<?php

namespace App\Http\Controllers\Admin;

use App\Models\Order;
use App\Models\Product;

class DashboardController
{
    public function index()
    {
        $productCount = Product::count();
        $orderCount = Order::count();
        $newOrderCount = Order::where('status', 'new')->count();
        $total = Order::sum('total_cents');
        $orders = Order::latest()->limit(5)->get();

        return view('admin.dashboard', compact('productCount', 'orderCount', 'newOrderCount', 'total', 'orders'));
    }
}

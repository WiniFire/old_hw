<?php

namespace App\Http\Controllers;

use App\Models\Product;
use App\Services\CartService;
use Illuminate\Http\Request;

class CartController
{
    public function index(CartService $cart)
    {
        $items = $cart->items();
        $total = $items->sum('subtotal');

        return view('cart.index', compact('items', 'total'));
    }

    public function add(Request $request, Product $product, CartService $cart)
    {
        $data = $request->validate(['quantity' => 'required|integer|min:1|max:1000']);
        $cart->add($product, (int) $data['quantity']);

        return back()->with('success', 'Товар добавлен в корзину.');
    }

    public function update(Request $request, Product $product, CartService $cart)
    {
        $data = $request->validate(['quantity' => 'required|integer|min:1|max:1000']);
        $cart->update($product, (int) $data['quantity']);

        return back()->with('success', 'Количество обновлено.');
    }

    public function remove(Product $product, CartService $cart)
    {
        $cart->remove($product->id);

        return back()->with('success', 'Товар удалён из корзины.');
    }
}

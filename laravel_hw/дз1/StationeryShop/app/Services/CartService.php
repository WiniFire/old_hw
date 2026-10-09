<?php

namespace App\Services;

use App\Models\Product;
use Illuminate\Support\Collection;
use Illuminate\Validation\ValidationException;

class CartService
{
    public function quantities(): array
    {
        return session('cart', []);
    }

    public function items(): Collection
    {
        $quantities = $this->quantities();
        $products = Product::whereIn('id', array_keys($quantities))->get()->keyBy('id');
        $items = collect();

        foreach ($quantities as $id => $quantity) {
            if (! isset($products[$id])) {
                unset($quantities[$id]);
                continue;
            }

            $product = $products[$id];
            $items->push(['product' => $product, 'quantity' => $quantity, 'subtotal' => $product->price_cents * $quantity]);
        }

        session(['cart' => $quantities]);

        return $items;
    }

    public function add(Product $product, int $quantity): void
    {
        $cart = $this->quantities();
        $newQuantity = ($cart[$product->id] ?? 0) + $quantity;
        $this->checkQuantity($product, $newQuantity);
        $cart[$product->id] = $newQuantity;
        session(['cart' => $cart]);
    }

    public function update(Product $product, int $quantity): void
    {
        $cart = $this->quantities();
        if (! isset($cart[$product->id])) {
            throw ValidationException::withMessages(['cart' => 'Товар не найден в корзине.']);
        }

        $this->checkQuantity($product, $quantity);
        $cart[$product->id] = $quantity;
        session(['cart' => $cart]);
    }

    public function remove(int $id): void
    {
        $cart = $this->quantities();
        unset($cart[$id]);
        session(['cart' => $cart]);
    }

    public function clear(): void
    {
        session()->forget('cart');
    }

    private function checkQuantity(Product $product, int $quantity): void
    {
        if ($quantity < 1 || $quantity > 1000 || $quantity > $product->stock) {
            throw ValidationException::withMessages(['cart' => 'Недостаточно товара «'.$product->name.'». На складе: '.$product->stock.'. Максимум в корзине: 1000 шт.']);
        }
    }
}

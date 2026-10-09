<?php

namespace App\Http\Controllers\Admin;

use App\Models\Product;
use Illuminate\Http\Request;

class ProductController
{
    public function index()
    {
        $products = Product::latest()->paginate(15);

        return view('admin.products.index', compact('products'));
    }

    public function create()
    {
        return view('admin.products.form', ['product' => new Product()]);
    }

    public function store(Request $request)
    {
        Product::create($this->validated($request));

        return redirect()->route('admin.products.index')->with('success', 'Товар создан.');
    }

    public function edit(Product $product)
    {
        return view('admin.products.form', compact('product'));
    }

    public function update(Request $request, Product $product)
    {
        $product->update($this->validated($request));

        return redirect()->route('admin.products.index')->with('success', 'Товар обновлён.');
    }

    public function destroy(Product $product)
    {
        $product->delete();

        return redirect()->route('admin.products.index')->with('success', 'Товар удалён из каталога.');
    }

    private function validated(Request $request): array
    {
        $data = $request->validate([
            'name' => 'required|string|max:100',
            'category' => 'required|string|max:60',
            'description' => 'required|string|max:3000',
            'price' => 'required|numeric|min:0.01|max:1000000',
            'stock' => 'required|integer|min:0|max:1000000',
        ]);

        $data['name'] = trim($data['name']);
        $data['category'] = trim($data['category']);
        $data['price_cents'] = (int) round((float) $data['price'] * 100);
        unset($data['price']);

        return $data;
    }
}

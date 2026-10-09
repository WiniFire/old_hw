<?php

namespace App\Http\Controllers;

use App\Models\Product;
use Illuminate\Http\Request;

class CatalogController
{
    public function index(Request $request)
    {
        $filters = $request->validate(['search' => 'nullable|string|max:100', 'category' => 'nullable|string|max:60']);
        $products = Product::query()
            ->when($filters['search'] ?? null, fn ($query, $search) => $query->where('name', 'like', '%'.$search.'%'))
            ->when($filters['category'] ?? null, fn ($query, $category) => $query->where('category', $category))
            ->orderBy('name')->paginate(12)->withQueryString();
        $categories = Product::select('category')->distinct()->orderBy('category')->pluck('category');

        return view('catalog.index', compact('products', 'categories'));
    }

    public function show(Product $product)
    {
        return view('catalog.show', compact('product'));
    }
}

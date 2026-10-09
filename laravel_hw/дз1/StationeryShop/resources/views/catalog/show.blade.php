@extends('layouts.app')
@section('title', $product->name)
@section('content')
    <a class="back-link" href="{{ route('catalog.index') }}">← В каталог</a>
    <div class="detail-grid">
        <div class="product-art large color-{{ $product->id % 4 }}"><span aria-hidden="true">{{ mb_substr($product->name, 0, 1) }}</span><small>{{ $product->category }}</small></div>
        <section class="panel product-detail"><p class="eyebrow">{{ $product->category }}</p><h1>{{ $product->name }}</h1><p class="description">{{ $product->description }}</p><p class="detail-price">{{ $product->formattedPrice() }}</p><p class="muted">Остаток: {{ $product->stock }} шт.</p><form class="inline-form" method="POST" action="{{ route('cart.add', $product) }}">@csrf<div class="field"><label for="quantity">Количество</label><input id="quantity" name="quantity" type="number" min="1" max="{{ min($product->stock, 1000) }}" value="1" required @disabled($product->stock === 0)></div><button class="button" type="submit" @disabled($product->stock === 0)>{{ $product->stock > 0 ? 'Добавить в корзину' : 'Нет в наличии' }}</button></form></section>
    </div>
@endsection

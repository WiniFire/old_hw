@extends('layouts.app')
@section('title', 'Каталог')
@section('content')
    <section class="hero">
        <div><p class="eyebrow">МАЛЕНЬКИЕ ВЕЩИ ДЛЯ БОЛЬШИХ ИДЕЙ</p><h1>Для ваших заметок,<br>планов и вдохновения.</h1><p>Любимые канцтовары — в одном месте.</p><a class="button" href="#catalog">Посмотреть каталог</a></div>
        <div class="hero-art" aria-hidden="true"><span class="paper paper-one"></span><span class="paper paper-two"></span><span class="pencil"></span><span class="art-caption">Запиши<br>свою идею.</span></div>
    </section>
    <div class="page-heading" id="catalog"><h2>Каталог товаров</h2><span class="muted">Найдено: {{ $products->total() }}</span></div>
    <form class="filters panel" action="{{ route('catalog.index') }}" method="GET">
        <div class="field"><label for="search">Название</label><input id="search" name="search" value="{{ request('search') }}" placeholder="Поиск товара" maxlength="100"></div>
        <div class="field"><label for="category">Категория</label><select id="category" name="category"><option value="">Все категории</option>@foreach($categories as $category)<option value="{{ $category }}" @selected(request('category') === $category)>{{ $category }}</option>@endforeach</select></div>
        <button class="button" type="submit">Найти</button><a class="button secondary" href="{{ route('catalog.index') }}">Сбросить</a>
    </form>
    <div class="product-grid">
        @forelse($products as $product)
            <article class="product-card">
                <a class="product-art color-{{ $product->id % 4 }}" href="{{ route('catalog.show', $product) }}" aria-label="{{ $product->name }}"><span aria-hidden="true">{{ mb_substr($product->name, 0, 1) }}</span><small>{{ $product->category }}</small></a>
                <div class="product-info"><p class="category-label">{{ $product->category }}</p><h3><a href="{{ route('catalog.show', $product) }}">{{ $product->name }}</a></h3><p class="muted stock">{{ $product->stock > 0 ? 'В наличии: '.$product->stock.' шт.' : 'Нет в наличии' }}</p><div class="card-bottom"><strong>{{ $product->formattedPrice() }}</strong><form method="POST" action="{{ route('cart.add', $product) }}">@csrf<input type="hidden" name="quantity" value="1"><button class="button compact" type="submit" @disabled($product->stock === 0)>В корзину</button></form></div></div>
            </article>
        @empty
            <div class="panel empty-state"><h3>Товары не найдены</h3><p class="muted">Попробуйте изменить поиск или категорию.</p></div>
        @endforelse
    </div>
    {{ $products->links('partials.pagination') }}
@endsection

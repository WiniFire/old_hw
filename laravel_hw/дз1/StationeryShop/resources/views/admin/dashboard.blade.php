@extends('layouts.app')
@section('title', 'Обзор магазина')
@section('content')
    <div class="page-heading"><h1>Обзор магазина</h1><a class="button" href="{{ route('admin.products.create') }}">Добавить товар</a></div><div class="stats-grid"><div class="panel stat"><span>Товаров</span><strong>{{ $productCount }}</strong></div><div class="panel stat"><span>Заказов</span><strong>{{ $orderCount }}</strong></div><div class="panel stat"><span>Новых заказов</span><strong>{{ $newOrderCount }}</strong></div><div class="panel stat"><span>Сумма заказов</span><strong>{{ number_format($total / 100, 2, ',', ' ') }} грн</strong></div></div><div class="page-heading"><h2>Последние заказы</h2><a href="{{ route('admin.orders.index') }}">Все заказы →</a></div>@include('partials.orders-table')
@endsection

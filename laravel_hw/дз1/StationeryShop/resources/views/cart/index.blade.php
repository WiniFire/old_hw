@extends('layouts.app')
@section('title', 'Корзина')
@section('content')
    <div class="page-heading"><h1>Ваша корзина</h1><a class="button secondary" href="{{ route('catalog.index') }}">Продолжить покупки</a></div>
    @if($items->isEmpty())
        <div class="panel empty-state"><h2>Пока здесь пусто</h2><p class="muted">Выберите что-нибудь для учёбы или творчества.</p><a class="button" href="{{ route('catalog.index') }}">Перейти в каталог</a></div>
    @else
        <div class="checkout-grid"><div class="panel table-wrap"><table><thead><tr><th>Товар</th><th>Количество</th><th>Сумма</th><th></th></tr></thead><tbody>
            @foreach($items as $item)
                <tr><td><a class="table-title" href="{{ route('catalog.show', $item['product']) }}">{{ $item['product']->name }}</a><small>{{ $item['product']->formattedPrice() }} / шт.</small></td><td><form class="quantity-form" method="POST" action="{{ route('cart.update', $item['product']) }}">@csrf @method('PATCH')<input type="number" name="quantity" min="1" max="1000" value="{{ $item['quantity'] }}" aria-label="Количество {{ $item['product']->name }}" required><button class="button secondary compact" type="submit">Обновить</button></form></td><td class="nowrap">{{ number_format($item['subtotal'] / 100, 2, ',', ' ') }} грн</td><td><form method="POST" action="{{ route('cart.remove', $item['product']) }}">@csrf @method('DELETE')<button class="text-button danger" type="submit">Удалить</button></form></td></tr>
            @endforeach
        </tbody></table></div><aside class="panel order-summary"><h2>Ваш заказ</h2><p class="muted">Товаров: {{ $items->sum('quantity') }}</p><div class="summary-total"><span>Итого</span><strong>{{ number_format($total / 100, 2, ',', ' ') }} грн</strong></div><a class="button full" href="{{ route('orders.create') }}">Оформить заказ</a></aside></div>
    @endif
@endsection

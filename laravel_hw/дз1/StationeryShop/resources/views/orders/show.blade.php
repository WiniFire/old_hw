@extends('layouts.app')
@section('title', 'Заказ №'.$order->id)
@section('content')
    <div class="panel confirmation"><span class="confirmation-mark" aria-hidden="true">✓</span><p class="eyebrow">СПАСИБО ЗА ПОКУПКУ</p><h1>Заказ №{{ $order->id }} принят</h1><p>Мы свяжемся с вами для подтверждения доставки.</p><span class="status-badge">{{ $order->statusLabel() }}</span><p class="muted">{{ $order->created_at->format('d.m.Y H:i') }}</p><p>Получатель: {{ $order->customer_name }}</p><p>Адрес: {{ $order->address }}</p></div>
    <div class="panel table-wrap"><table><thead><tr><th>Товар</th><th>Количество</th><th>Сумма</th></tr></thead><tbody>@foreach($order->items as $item)<tr><td>{{ $item->name }}</td><td>{{ $item->quantity }}</td><td>{{ $item->formattedSubtotal() }}</td></tr>@endforeach</tbody></table><div class="summary-total"><span>Итого</span><strong>{{ $order->formattedTotal() }}</strong></div></div><p><a class="button" href="{{ route('catalog.index') }}">Вернуться к покупкам</a></p>
@endsection

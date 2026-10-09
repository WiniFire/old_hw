@extends('layouts.app')
@section('title', 'Заказы')
@section('content')
    <h1>Заказы</h1>@include('partials.orders-table'){{ $orders->links('partials.pagination') }}
@endsection

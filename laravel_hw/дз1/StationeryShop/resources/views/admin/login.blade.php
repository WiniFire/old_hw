@extends('layouts.app')
@section('title', 'Вход администратора')
@section('content')
    <div class="login-box panel"><p class="eyebrow">УПРАВЛЕНИЕ МАГАЗИНОМ</p><h1>Вход администратора</h1><form class="stacked-form" method="POST" action="{{ route('admin.login.store') }}">@csrf<div class="field"><label for="email">Email</label><input id="email" name="email" type="email" value="{{ old('email') }}" autocomplete="username" required></div><div class="field"><label for="password">Пароль</label><input id="password" name="password" type="password" autocomplete="current-password" required></div><button class="button" type="submit">Войти</button></form></div>
@endsection

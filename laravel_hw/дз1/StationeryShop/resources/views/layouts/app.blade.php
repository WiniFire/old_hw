<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@yield('title', 'КанцМаркет') · КанцМаркет</title>
    <link rel="stylesheet" href="{{ asset('css/shop.css') }}">
    <script src="{{ asset('js/shop.js') }}" defer></script>
</head>
<body>
    <header class="site-header">
        <div class="container header-inner">
            <a class="brand" href="{{ route('catalog.index') }}"><span class="brand-mark">К</span> КанцМаркет</a>
            <nav aria-label="Основная навигация">
                <a href="{{ route('catalog.index') }}" @class(['active' => request()->routeIs('catalog.*')])>Каталог</a>
                <a href="{{ route('cart.index') }}" @class(['active' => request()->routeIs('cart.*')])>Корзина <span class="nav-count">{{ array_sum(session('cart', [])) }}</span></a>
                <a href="{{ route('admin.dashboard') }}" @class(['active' => request()->routeIs('admin.*')])>Админ-панель</a>
            </nav>
        </div>
    </header>
    <main class="container main-content">
        @if(auth()->user()?->is_admin && request()->routeIs('admin.*'))
            <div class="admin-nav">
                <a href="{{ route('admin.dashboard') }}">Обзор</a>
                <a href="{{ route('admin.products.index') }}">Товары</a>
                <a href="{{ route('admin.orders.index') }}">Заказы</a>
                <form method="POST" action="{{ route('admin.logout') }}">@csrf<button class="text-button" type="submit">Выйти</button></form>
            </div>
        @endif
        @if(session('success'))<div class="alert success" role="status">{{ session('success') }}</div>@endif
        @if(session('error'))<div class="alert error" role="alert">{{ session('error') }}</div>@endif
        @if($errors->any())
            <div class="alert error" role="alert"><ul>@foreach($errors->all() as $error)<li>{{ $error }}</li>@endforeach</ul></div>
        @endif
        @yield('content')
    </main>
    <footer class="site-footer"><div class="container">КанцМаркет · Всё для учёбы, работы и творчества</div></footer>
</body>
</html>

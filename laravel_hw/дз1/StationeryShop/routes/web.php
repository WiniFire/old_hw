<?php

use App\Http\Controllers\Admin;
use App\Http\Controllers\CartController;
use App\Http\Controllers\CatalogController;
use App\Http\Controllers\OrderController;
use Illuminate\Support\Facades\Route;

Route::get('/', [CatalogController::class, 'index'])->name('catalog.index');
Route::get('/products/{product}', [CatalogController::class, 'show'])->name('catalog.show');
Route::get('/cart', [CartController::class, 'index'])->name('cart.index');
Route::post('/cart/{product}', [CartController::class, 'add'])->name('cart.add');
Route::patch('/cart/{product}', [CartController::class, 'update'])->name('cart.update');
Route::delete('/cart/{product}', [CartController::class, 'remove'])->name('cart.remove');
Route::get('/checkout', [OrderController::class, 'create'])->name('orders.create');
Route::post('/checkout', [OrderController::class, 'store'])->block(10, 10)->name('orders.store');
Route::get('/orders/{token}', [OrderController::class, 'show'])->whereUuid('token')->name('orders.show');

Route::get('/admin/login', [Admin\AuthController::class, 'create'])->name('admin.login');
Route::post('/admin/login', [Admin\AuthController::class, 'store'])->middleware('throttle:5,1')->name('admin.login.store');

Route::prefix('admin')->name('admin.')->middleware('admin')->group(function () {
    Route::get('/', [Admin\DashboardController::class, 'index'])->name('dashboard');
    Route::post('/logout', [Admin\AuthController::class, 'destroy'])->name('logout');
    Route::resource('products', Admin\ProductController::class)->except('show');
    Route::get('/orders', [Admin\OrderController::class, 'index'])->name('orders.index');
    Route::get('/orders/{order}', [Admin\OrderController::class, 'show'])->name('orders.show');
    Route::patch('/orders/{order}', [Admin\OrderController::class, 'update'])->name('orders.update');
});

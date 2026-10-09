<?php

namespace Database\Seeders;

use App\Models\Product;
use App\Models\User;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    public function run(): void
    {
        User::firstOrCreate(['email' => 'admin@stationery.local'], [
            'name' => 'Администратор',
            'password' => 'Admin12345!',
            'is_admin' => true,
        ]);

        $products = [
            ['Тетрадь 48 листов', 'Тетради', 'Тетрадь в клетку с плотной обложкой. Для учёбы и ежедневных заметок.', 4500, 40],
            ['Блокнот А5', 'Тетради', 'Компактный блокнот на 80 страниц с эластичной резинкой.', 12000, 25],
            ['Ручка шариковая', 'Письменные принадлежности', 'Синяя шариковая ручка с удобным корпусом.', 2000, 80],
            ['Карандаш HB', 'Письменные принадлежности', 'Графитовый карандаш средней твёрдости.', 1500, 60],
            ['Набор маркеров', 'Рисование', 'Набор из шести цветных маркеров для рисунков и заметок.', 18000, 15],
            ['Акварель 12 цветов', 'Рисование', 'Краски для школьных и домашних творческих занятий.', 15000, 18],
            ['Папка А4', 'Папки', 'Папка на кнопке для документов формата А4.', 7000, 30],
            ['Пенал', 'Аксессуары', 'Текстильный пенал на молнии для любимых канцелярских мелочей.', 21000, 12],
        ];

        foreach ($products as [$name, $category, $description, $price, $stock]) {
            Product::firstOrCreate(['name' => $name], [
                'category' => $category,
                'description' => $description,
                'price_cents' => $price,
                'stock' => $stock,
            ]);
        }
    }
}

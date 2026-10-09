<?php

return [
    'required' => 'Заполните поле «:attribute».',
    'string' => 'Поле «:attribute» должно быть строкой.',
    'email' => 'Введите корректный email.',
    'integer' => 'Поле «:attribute» должно содержать целое число.',
    'numeric' => 'Поле «:attribute» должно содержать число.',
    'regex' => 'Проверьте формат поля «:attribute».',
    'in' => 'Выбрано недопустимое значение поля «:attribute».',
    'min' => ['numeric' => 'Минимальное значение поля «:attribute»: :min.', 'string' => 'Поле «:attribute» должно содержать не менее :min символов.'],
    'max' => ['numeric' => 'Максимальное значение поля «:attribute»: :max.', 'string' => 'Поле «:attribute» должно содержать не более :max символов.'],
    'attributes' => [
        'name' => 'Название', 'category' => 'Категория', 'description' => 'Описание',
        'price' => 'Цена', 'stock' => 'Остаток', 'quantity' => 'Количество',
        'customer_name' => 'Имя', 'email' => 'Email', 'phone' => 'Телефон',
        'address' => 'Адрес', 'password' => 'Пароль', 'status' => 'Статус', 'search' => 'Поиск',
    ],
];

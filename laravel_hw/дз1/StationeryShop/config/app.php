<?php

return [
    'name' => env('APP_NAME', 'КанцМаркет'),
    'env' => env('APP_ENV', 'local'),
    'debug' => (bool) env('APP_DEBUG', false),
    'url' => env('APP_URL', 'http://localhost'),
    'timezone' => env('APP_TIMEZONE', 'Europe/Kyiv'),
    'locale' => env('APP_LOCALE', 'ru'),
    'fallback_locale' => 'en',
    'key' => env('APP_KEY'),
    'cipher' => 'AES-256-CBC',
    'previous_keys' => [],
    'maintenance' => ['driver' => 'file'],
];

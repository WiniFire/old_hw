<?php

chdir(__DIR__);

foreach (['bootstrap/cache', 'storage/logs', 'storage/framework/cache/data', 'storage/framework/sessions', 'storage/framework/views'] as $directory) {
    if (! is_dir($directory)) mkdir($directory, 0755, true);
}

if (! file_exists('.env')) copy('.env.example', '.env');
if (! file_exists('database/database.sqlite')) touch('database/database.sqlite');

foreach (['key:generate --ansi', 'migrate --force'] as $command) {
    if (str_starts_with($command, 'key:generate') && preg_match('/^APP_KEY=.+$/m', file_get_contents('.env'))) continue;
    passthru(escapeshellarg(PHP_BINARY).' artisan '.$command, $code);
    if ($code !== 0) exit($code);
}

require __DIR__.'/vendor/autoload.php';
$app = require __DIR__.'/bootstrap/app.php';
$app->make(Illuminate\Contracts\Console\Kernel::class)->bootstrap();
if (! App\Models\User::exists()) {
    Illuminate\Support\Facades\Artisan::call('db:seed', ['--force' => true]);
    echo Illuminate\Support\Facades\Artisan::output();
}

echo "Магазин готов. Запустите start.bat или php artisan serve.\n";

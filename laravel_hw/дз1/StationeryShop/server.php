<?php

$publicPath = __DIR__ . '/public';
$uri = rawurldecode(parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH) ?? '/');
$file = realpath($publicPath . $uri);
$root = realpath($publicPath) . DIRECTORY_SEPARATOR;

if ($uri !== '/' && $file !== false && str_starts_with($file, $root) && is_file($file)) {
    return false;
}

require $publicPath . '/index.php';

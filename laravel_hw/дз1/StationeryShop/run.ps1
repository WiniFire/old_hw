param([switch]$PrepareOnly)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$env:PSModulePath = (Join-Path $PSHOME 'Modules') + ';' + $env:PSModulePath
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Get-ShopDownload {
    param([string]$Uri, [string]$Destination, [string]$Sha256)

    if (Test-Path -LiteralPath $Destination) {
        if ($Sha256 -and (Get-FileHash -LiteralPath $Destination -Algorithm SHA256).Hash -eq $Sha256) { return }
        if (-not $Sha256 -and (Get-Content -LiteralPath $Destination -Raw) -match '-----BEGIN CERTIFICATE-----') { return }
    }

    $shopPartial = $Destination + '.download'
    $shopCurl = Get-Command curl.exe -ErrorAction SilentlyContinue
    for ($shopAttempt = 1; $shopAttempt -le 3; $shopAttempt++) {
        try {
            if ($shopCurl) {
                & $shopCurl.Source --fail --location --silent --show-error --retry 2 --retry-delay 2 --connect-timeout 20 --max-time 180 --output $shopPartial $Uri
                if ($LASTEXITCODE -ne 0) { throw 'Не удалось завершить загрузку.' }
            } else {
                Invoke-WebRequest -UseBasicParsing -Uri $Uri -OutFile $shopPartial -TimeoutSec 180
            }
            if ($Sha256) {
                if ((Get-FileHash -LiteralPath $shopPartial -Algorithm SHA256).Hash -ne $Sha256) { throw 'Контрольная сумма файла не совпадает.' }
            } elseif ((Get-Content -LiteralPath $shopPartial -Raw) -notmatch '-----BEGIN CERTIFICATE-----') {
                throw 'Не удалось проверить сертификаты.'
            }
            Move-Item -LiteralPath $shopPartial -Destination $Destination -Force
            return
        } catch {
            if ($shopAttempt -eq 3) { throw }
            Write-Host ('Повторная попытка загрузки: ' + ($shopAttempt + 1))
            Start-Sleep -Seconds 2
        }
    }
}

Set-Location -LiteralPath $PSScriptRoot
$shopTools = Join-Path $PSScriptRoot '.runtime'
$shopPhpDir = Join-Path $shopTools 'php'
$shopPhp = Join-Path $shopPhpDir 'php.exe'
$shopComposer = Join-Path $shopTools 'composer.phar'
New-Item -ItemType Directory -Path $shopTools -Force | Out-Null

if (-not (Test-Path -LiteralPath $shopPhp)) {
    Write-Host 'Подготовка PHP...'
    $shopZip = Join-Path $shopTools 'php.zip'
    Get-ShopDownload -Uri 'https://downloads.php.net/~windows/releases/php-8.5.11-nts-Win32-vs17-x64.zip' -Destination $shopZip -Sha256 '0ea96e0d2b9b737a6036f05cf4e95c49313faa6d0f27bd97edb2742503f0c043'
    Expand-Archive -LiteralPath $shopZip -DestinationPath $shopPhpDir -Force
}

Write-Host 'Проверка Composer...'
Get-ShopDownload -Uri 'https://getcomposer.org/download/2.10.3/composer.phar' -Destination $shopComposer -Sha256 '7a2d379d5b8ffdaa028580ef26494c36d2feef4b178d3dd1473a4dbc5e17c8d6'

$shopCa = Join-Path $shopTools 'cacert.pem'
Get-ShopDownload -Uri 'https://curl.se/ca/cacert.pem' -Destination $shopCa

$shopExtensions = 'ext'
$shopCertificate = '.runtime/cacert.pem'
$shopIni = @(
    ('extension_dir="' + $shopExtensions + '"'),
    'extension=curl', 'extension=fileinfo', 'extension=mbstring', 'extension=openssl',
    'extension=pdo_sqlite', 'extension=sqlite3', 'extension=zip', 'extension=intl',
    'date.timezone=Europe/Kyiv', 'memory_limit=512M',
    ('curl.cainfo="' + $shopCertificate + '"'),
    ('openssl.cafile="' + $shopCertificate + '"')
) -join [Environment]::NewLine
[System.IO.File]::WriteAllText((Join-Path $shopPhpDir 'php.ini'), $shopIni, [System.Text.UTF8Encoding]::new($false))
$env:PATH = $shopPhpDir + ';' + $env:PATH
$env:COMPOSER_HOME = Join-Path $shopTools 'composer-home'
$env:COMPOSER_CACHE_DIR = Join-Path $shopTools 'composer-cache'

if (-not (Test-Path -LiteralPath 'vendor\autoload.php')) {
    & $shopPhp $shopComposer install --no-dev --prefer-dist --no-interaction
    if ($LASTEXITCODE -ne 0) { throw 'Не удалось установить зависимости.' }
}

& $shopPhp 'setup.php'
if ($LASTEXITCODE -ne 0) { throw 'Не удалось подготовить базу данных.' }
Write-Host 'Откройте http://127.0.0.1:8000/'
Write-Host 'Админ-панель: /admin'
if ($PrepareOnly) { exit 0 }
& $shopPhp '-S' '127.0.0.1:8000' '-t' 'public' 'server.php'

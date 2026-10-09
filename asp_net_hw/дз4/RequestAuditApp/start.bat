@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0"
set "shopDotnet=%ProgramFiles%\dotnet\dotnet.exe"
if exist "%shopDotnet%" goto launch
set "shopDotnet=dotnet"
where dotnet >nul 2>nul
if not errorlevel 1 goto launch
echo .NET SDK 10 was not found. Install the SDK and try again.
pause
exit /b 1
:launch
"%shopDotnet%" run --project "%~dp0RequestAuditApp.csproj" %*
set "shopExit=%errorlevel%"
pause
exit /b %shopExit%


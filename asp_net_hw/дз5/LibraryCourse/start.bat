@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0"
set "libraryDotnet=%ProgramFiles%\dotnet\dotnet.exe"
if exist "%libraryDotnet%" goto launch
set "libraryDotnet=dotnet"
where dotnet >nul 2>nul
if not errorlevel 1 goto launch
echo Install .NET SDK 10 to run this project.
pause
exit /b 1
:launch
echo URL: http://localhost:5085
echo Login: admin   Password: Library123!
"%libraryDotnet%" run --project "%~dp0LibraryCourse.csproj"
pause
@echo off
cd /d %~dp0
node script.js %*
:menu
cls
echo =========================================================================================================
echo ======================================== *Structure Transformer* ========================================
echo =========================================================================================================
color 0A
type banner.txt
color 0C
echo.
echo.
echo ======================================== Choose transformation: =========================================
echo.
color 07
echo(                                         ^> 1. Reset
echo(                                         ^> 2. Rotate-Z 90*
echo(                                         ^> 3. Rotate-X 90*
echo(                                         ^> 4. Rotate-Y 90*
echo(                                         ^> 5. Mirror Horizontally
echo(                                         ^> 6. Mirror Vertically
echo(                                         ^> 7. Exit
echo.
set /p choice=                               "Enter your choice: "

if "%choice%"=="1" (
    node MultiConverter.js reset
    goto menu
)
if "%choice%"=="2" (
    node MultiConverter.js rotatez
    goto menu
)
if "%choice%"=="3" (
    node MultiConverter.js rotatex
    goto menu
)
if "%choice%"=="4" (
    node MultiConverter.js rotatey
    goto menu
)
if "%choice%"=="5" (
    node MultiConverter.js mirrorh
    goto menu
)
if "%choice%"=="6" (
    node MultiConverter.js mirrorv
    goto menu
)
if "%choice%"=="7" (
    exit
)

echo Invalid choice. Try again.
pause
goto menu
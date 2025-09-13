@echo off
title Structure Converter
:menu
cls
echo ===================================
echo      Structure Converter Menu
echo ===================================
echo.
echo 1. No Transformations
echo 2. Rotate 90 Degrees
echo 3. Mirror Vertically
echo 4. Mirror Horizontally
echo 5. Mirror Horizontally + Vertically 
echo 6. Rotate + Mirror Horizontally
echo 7. Rotate + Mirror Vertically
echo 8. Exit
echo.
set /p choice="Choose an option (1-8): "

if "%choice%"=="1" (
    node MultiConverter.js normal
    pause
    goto menu
)
if "%choice%"=="2" (
    node MultiConverter.js rotate
    pause
    goto menu
)
if "%choice%"=="3" (
    node MultiConverter.js mirrorV
    pause
    goto menu
)
if "%choice%"=="4" (
    node MultiConverter.js mirrorH
    pause
    goto menu
)
if "%choice%"=="5" (
    node MultiConverter.js mirrorVH
    pause
    goto menu
)
if "%choice%"=="6" (
    node MultiConverter.js rotateMirrorH
    pause
    goto menu
)
if "%choice%"=="7" (
    node MultiConverter.js rotateMirrorV
    pause
    goto menu
)
if "%choice%"=="8" (
    exit
)

echo Invalid choice. Try again.
pause
goto menu
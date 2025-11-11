@echo off
echo Compilando programa de prueba de conexion...

REM Compilar el programa de prueba
csc /reference:System.Data.dll /reference:System.Configuration.dll PruebaConexion.cs

if errorlevel 1 (
  echo ? Error al compilar
    pause
    exit
)

echo.
echo ? Compilacion exitosa! Ejecutando prueba...
echo.

REM Ejecutar la prueba
PruebaConexion.exe

REM Limpiar archivos temporales
del PruebaConexion.exe

echo.
echo Limpieza completada.
pause
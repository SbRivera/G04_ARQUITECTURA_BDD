@echo off
echo Respaldando Web.config...
copy Web.config Web.config.backup

echo.
echo Actualizando cadena de conexion...

REM Usar PowerShell para reemplazar la cadena de conexion
powershell -Command "(Get-Content 'Web.config') -replace 'Server=JOIS\\MSSQLSERVER01; Initial Catalog=EUREKABANK; Integrated Security=True; TrustServerCertificate=True', 'Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\EUREKABANK.mdf;Initial Catalog=EUREKABANK;Integrated Security=True;TrustServerCertificate=True' | Set-Content 'Web.config'"

echo.
echo ? Cadena de conexion actualizada
echo.
echo ANTES: Server=JOIS\MSSQLSERVER01
echo AHORA: Server=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\EUREKABANK.mdf
echo.
echo Archivo de respaldo: Web.config.backup
echo.
pause
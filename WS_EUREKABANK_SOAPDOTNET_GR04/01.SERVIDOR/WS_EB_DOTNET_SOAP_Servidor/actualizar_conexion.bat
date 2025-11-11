@echo off
echo Actualizando cadena de conexion en Web.config...

REM Crear backup del archivo original
copy Web.config Web.config.backup

REM Crear nuevo Web.config con la cadena de conexion correcta
(
echo ^<?xml version="1.0" encoding="utf-8"?^>
echo ^<configuration^>
echo   ^<connectionStrings^>
echo     ^<add name="EUREKABANK" connectionString="Server=^(LocalDB^)\MSSQLLocalDB;AttachDbFilename=^|DataDirectory^|\EUREKABANK.mdf;Initial Catalog=EUREKABANK;Integrated Security=True;TrustServerCertificate=True" providerName="System.Data.SqlClient" /^>
echo   ^</connectionStrings^>
echo   ^<system.web^>
echo ^<compilation debug="true" targetFramework="4.7.2" /^>
echo     ^<httpRuntime targetFramework="4.7.2" /^>
echo     ^<webServices^>
echo     ^<protocols^>
echo         ^<add name="HttpGet"/^>
echo         ^<add name="HttpPost"/^>
echo   ^<add name="Documentation"/^>
echo       ^</protocols^>
echo   ^</webServices^>
echo   ^</system.web^>
echo ^</configuration^>
) > Web.config

echo.
echo ? Web.config actualizado correctamente
echo.
echo Cadena de conexion anterior: Server=JOIS\MSSQLSERVER01
echo Cadena de conexion nueva: Server=^(LocalDB^)\MSSQLLocalDB;AttachDbFilename=^|DataDirectory^|\EUREKABANK.mdf
echo.
echo Se creo un backup en Web.config.backup
echo.
pause
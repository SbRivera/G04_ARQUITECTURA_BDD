@echo off
echo Aplicando version 2 de la cadena de conexion...
copy Web_config_v2.xml Web.config

echo.
echo ? Web.config actualizado con cadena simplificada para LocalDB
echo.
echo NUEVA CADENA DE CONEXION:
echo Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=EUREKABANK;Integrated Security=True;TrustServerCertificate=True
echo.
pause
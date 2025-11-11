@echo off
echo Creando backup de Web.config...
copy Web.config Web.config.backup

echo.
echo Reemplazando Web.config...
copy Web_config_nuevo.xml Web.config

echo.
echo ? Web.config actualizado correctamente!
echo.
echo CADENA DE CONEXION ANTERIOR:
echo Server=JOIS\MSSQLSERVER01; Initial Catalog=EUREKABANK; Integrated Security=True; TrustServerCertificate=True
echo.
echo CADENA DE CONEXION NUEVA:
echo Server=(LocalDB)\MSSQLLocalDB;AttachDbFilename=^|DataDirectory^|\EUREKABANK.mdf;Initial Catalog=EUREKABANK;Integrated Security=True;TrustServerCertificate=True
echo.
echo Esto deberia resolver el problema de conexion a la base de datos.
echo Archivo de respaldo guardado como: Web.config.backup
echo.
pause
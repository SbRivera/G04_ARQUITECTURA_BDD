# WS_EUREKABANK_RESTFULLNOTNET_GR04

Solución estilo Visual Studio 2022 compuesta por:

- 01.SERVIDOR/WS_EB_DOTNET_REST_Servidor (ASP.NET Core Web API .NET 8)
- 02.CLICON/EB_DOTNET_REST_CliCon (Consola .NET 8)

## Endpoints principales

Base: http://localhost:5043 (o https://localhost:7043)

- GET api/CoreBancario/cuentas/{cuenta}/movimientos
- POST api/CoreBancario/deposito { cuenta, importe }
- POST api/CoreBancario/retiro { cuenta, importe }
- POST api/CoreBancario/transferencia { origen, destino, importe }

Swagger/UI: /swagger

## Configuración de BD

Se usa SQL Server LocalDB y la BD `EUREKABANK` (misma que el proyecto SOAP .NET). Cambia la cadena en `appsettings.json` si es necesario.

## Cómo ejecutar (PowerShell)

Servidor (API):

```powershell
cd WS_EUREKABANK_RESTFULLNOTNET_GR04\01.SERVIDOR\WS_EB_DOTNET_REST_Servidor
$env:ASPNETCORE_URLS="http://localhost:5043;https://localhost:7043"
dotnet run
```

Cliente consola:

```powershell
cd WS_EUREKABANK_RESTFULLNOTNET_GR04\02.CLICON\EB_DOTNET_REST_CliCon
$env:EB_API_BASE="http://localhost:5043/"
dotnet run
```

Si no tienes el SDK .NET 8, instálalo desde https://dotnet.microsoft.com/download/dotnet/8.0.

## Notas

- La tabla Movimiento debe existir con columnas compatibles: Id (int identity), Fecha (datetime), Tipo (varchar), CuentaOrigen (varchar), CuentaDestino (varchar, null), Importe (decimal), Moneda (varchar).
- La lógica de saldo/validación es mínima y de ejemplo; puedes ampliarla (validaciones de cuenta, saldos, etc.).

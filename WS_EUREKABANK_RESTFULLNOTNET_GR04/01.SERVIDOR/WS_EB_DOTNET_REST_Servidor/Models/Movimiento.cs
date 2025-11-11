namespace WS_EB_DOTNET_REST_Servidor.Models;

// Mapea columnas reales de la tabla Movimiento del script:
// chr_cuencodigo (char8), int_movinumero (int), dtt_movifecha (datetime),
// chr_emplcodigo (char4), chr_tipocodigo (char3), dec_moviimporte (money), chr_cuenreferencia (char8 nullable)
public class Movimiento
{
    public string CuentaCodigo { get; set; } = string.Empty; // chr_cuencodigo
    public int Numero { get; set; } // int_movinumero (contador por cuenta)
    public DateTime Fecha { get; set; } // dtt_movifecha
    public string EmpleadoCodigo { get; set; } = string.Empty; // chr_emplcodigo (usuario operación)
    public string TipoCodigo { get; set; } = string.Empty; // chr_tipocodigo (003 depósito, 004 retiro, 008/009 transferencia)
    public decimal Importe { get; set; } // dec_moviimporte
    public string? ReferenciaCuenta { get; set; } // chr_cuenreferencia (cuenta destino en transferencias)
}

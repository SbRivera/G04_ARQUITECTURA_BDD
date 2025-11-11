namespace WS_EB_DOTNET_REST_Servidor.Models;

public class Movimiento
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty; // DEP/RET/TRA
    public string CuentaOrigen { get; set; } = string.Empty;
    public string? CuentaDestino { get; set; }
    public decimal Importe { get; set; }
    public string Moneda { get; set; } = "USD";
}

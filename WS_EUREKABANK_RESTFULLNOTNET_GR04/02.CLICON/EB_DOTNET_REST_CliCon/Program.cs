using System.Net.Http.Json;
using System.Text.Json;

static class Api
{
    private static readonly HttpClient http = new HttpClient
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("EB_API_BASE") ?? "http://localhost:5043/")
    };

    public static async Task ListarMovimientosAsync()
    {
        Console.Write("Cuenta: ");
        var cuenta = Console.ReadLine()?.Trim();
        var movs = await http.GetFromJsonAsync<List<Movimiento>>($"api/CoreBancario/cuentas/{cuenta}/movimientos");
        Console.WriteLine(JsonSerializer.Serialize(movs, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static async Task DepositoAsync()
    {
        Console.Write("Cuenta: ");
        var cuenta = Console.ReadLine()?.Trim();
        Console.Write("Importe: ");
        var importe = decimal.Parse(Console.ReadLine()!);
        var resp = await http.PostAsJsonAsync("api/CoreBancario/deposito", new { cuenta, importe });
        Console.WriteLine(await resp.Content.ReadAsStringAsync());
    }

    public static async Task RetiroAsync()
    {
        Console.Write("Cuenta: ");
        var cuenta = Console.ReadLine()?.Trim();
        Console.Write("Importe: ");
        var importe = decimal.Parse(Console.ReadLine()!);
        var resp = await http.PostAsJsonAsync("api/CoreBancario/retiro", new { cuenta, importe });
        Console.WriteLine(await resp.Content.ReadAsStringAsync());
    }

    public static async Task TransferenciaAsync()
    {
        Console.Write("Cuenta Origen: ");
        var origen = Console.ReadLine()?.Trim();
        Console.Write("Cuenta Destino: ");
        var destino = Console.ReadLine()?.Trim();
        Console.Write("Importe: ");
        var importe = decimal.Parse(Console.ReadLine()!);
        var resp = await http.PostAsJsonAsync("api/CoreBancario/transferencia", new { origen, destino, importe });
        Console.WriteLine(await resp.Content.ReadAsStringAsync());
    }
}

public record Movimiento(int Id, DateTime Fecha, string Tipo, string CuentaOrigen, string? CuentaDestino, decimal Importe, string Moneda);

class Program
{
    static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("\n--- EB REST CliCon ---");
            Console.WriteLine("1) Listar Movimientos");
            Console.WriteLine("2) Depósito");
            Console.WriteLine("3) Retiro");
            Console.WriteLine("4) Transferencia");
            Console.WriteLine("0) Salir");
            Console.Write("Opción: ");
            var k = Console.ReadLine();
            try
            {
                switch (k)
                {
                    case "1": await Api.ListarMovimientosAsync(); break;
                    case "2": await Api.DepositoAsync(); break;
                    case "3": await Api.RetiroAsync(); break;
                    case "4": await Api.TransferenciaAsync(); break;
                    case "0": return;
                    default: Console.WriteLine("Opción inválida"); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

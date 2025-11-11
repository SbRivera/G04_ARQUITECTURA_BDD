using Microsoft.Data.SqlClient;
using WS_EB_DOTNET_REST_Servidor.Models;

namespace WS_EB_DOTNET_REST_Servidor.Services;

public class EurekaService
{
    private readonly IConfiguration _config;
    public EurekaService(IConfiguration config)
    {
        _config = config;
    }

    private SqlConnection GetConnection()
    {
        return new SqlConnection(_config.GetConnectionString("EUREKABANK")!);
    }

    public IEnumerable<Movimiento> ListarMovimientos(string cuenta)
    {
        var lista = new List<Movimiento>();
        using var cn = GetConnection();
        using var cmd = new SqlCommand("SELECT Id, Fecha, Tipo, CuentaOrigen, CuentaDestino, Importe, Moneda FROM Movimiento WHERE CuentaOrigen=@cuenta OR CuentaDestino=@cuenta ORDER BY Fecha DESC", cn);
        cmd.Parameters.AddWithValue("@cuenta", cuenta);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new Movimiento
            {
                Id = dr.GetInt32(0),
                Fecha = dr.GetDateTime(1),
                Tipo = dr.GetString(2),
                CuentaOrigen = dr.GetString(3),
                CuentaDestino = dr.IsDBNull(4) ? null : dr.GetString(4),
                Importe = dr.GetDecimal(5),
                Moneda = dr.GetString(6)
            });
        }
        return lista;
    }

    public Movimiento RegistrarDeposito(string cuenta, decimal importe)
    {
        using var cn = GetConnection();
        using var cmd = new SqlCommand("INSERT INTO Movimiento(Fecha,Tipo,CuentaOrigen,Importe,Moneda) OUTPUT INSERTED.Id VALUES(GETDATE(),'DEP',@cuenta,@importe,'USD')", cn);
        cmd.Parameters.AddWithValue("@cuenta", cuenta);
        cmd.Parameters.AddWithValue("@importe", importe);
        cn.Open();
        int id = (int)cmd.ExecuteScalar()!;
        return new Movimiento { Id = id, Fecha = DateTime.Now, Tipo = "DEP", CuentaOrigen = cuenta, Importe = importe, Moneda = "USD" };
    }

    public Movimiento RegistrarRetiro(string cuenta, decimal importe)
    {
        using var cn = GetConnection();
        using var cmd = new SqlCommand("INSERT INTO Movimiento(Fecha,Tipo,CuentaOrigen,Importe,Moneda) OUTPUT INSERTED.Id VALUES(GETDATE(),'RET',@cuenta,@importe,'USD')", cn);
        cmd.Parameters.AddWithValue("@cuenta", cuenta);
        cmd.Parameters.AddWithValue("@importe", importe);
        cn.Open();
        int id = (int)cmd.ExecuteScalar()!;
        return new Movimiento { Id = id, Fecha = DateTime.Now, Tipo = "RET", CuentaOrigen = cuenta, Importe = importe, Moneda = "USD" };
    }

    public IEnumerable<Movimiento> RegistrarTransferencia(string origen, string destino, decimal importe)
    {
        using var cn = GetConnection();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            // Retiro
            using var cmdRet = new SqlCommand("INSERT INTO Movimiento(Fecha,Tipo,CuentaOrigen,CuentaDestino,Importe,Moneda) OUTPUT INSERTED.Id VALUES(GETDATE(),'TRA',@origen,@destino,@importe,'USD')", cn, tx);
            cmdRet.Parameters.AddWithValue("@origen", origen);
            cmdRet.Parameters.AddWithValue("@destino", destino);
            cmdRet.Parameters.AddWithValue("@importe", importe);
            int idRet = (int)cmdRet.ExecuteScalar()!;

            // Deposito (registrar como transferencia también pero invertida si se desea duplicado) Opcional: solo un registro
            var movimientos = new List<Movimiento>
            {
                new Movimiento { Id = idRet, Fecha = DateTime.Now, Tipo = "TRA", CuentaOrigen = origen, CuentaDestino = destino, Importe = importe, Moneda = "USD" }
            };
            tx.Commit();
            return movimientos;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}

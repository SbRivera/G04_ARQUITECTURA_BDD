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

    // Obtiene movimientos de la cuenta siguiendo el esquema real.
    public IEnumerable<Movimiento> ListarMovimientos(string cuenta)
    {
        var lista = new List<Movimiento>();
        using var cn = GetConnection();
        using var cmd = new SqlCommand(@"SELECT chr_cuencodigo, int_movinumero, dtt_movifecha, chr_emplcodigo, chr_tipocodigo, dec_moviimporte, chr_cuenreferencia
                                          FROM Movimiento WHERE chr_cuencodigo = @cuenta OR chr_cuenreferencia = @cuenta ORDER BY dtt_movifecha DESC, int_movinumero DESC", cn);
        cmd.Parameters.AddWithValue("@cuenta", cuenta);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            lista.Add(new Movimiento
            {
                CuentaCodigo = dr.GetString(0),
                Numero = dr.GetInt32(1),
                Fecha = dr.GetDateTime(2),
                EmpleadoCodigo = dr.GetString(3),
                TipoCodigo = dr.GetString(4),
                Importe = (decimal)dr.GetDecimal(5),
                ReferenciaCuenta = dr.IsDBNull(6) ? null : dr.GetString(6)
            });
        }
        return lista;
    }

    private int ObtenerSiguienteMovimiento(SqlConnection cn, SqlTransaction tx, string cuenta)
    {
        using var cmd = new SqlCommand("SELECT ISNULL(MAX(int_movinumero),0)+1 FROM Movimiento WHERE chr_cuencodigo=@c", cn, tx);
        cmd.Parameters.AddWithValue("@c", cuenta);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private void ActualizarSaldo(SqlConnection cn, SqlTransaction tx, string cuenta, decimal delta)
    {
        using var cmd = new SqlCommand("UPDATE Cuenta SET dec_cuensaldo = dec_cuensaldo + @d WHERE chr_cuencodigo=@c", cn, tx);
        cmd.Parameters.AddWithValue("@d", delta);
        cmd.Parameters.AddWithValue("@c", cuenta);
        if (cmd.ExecuteNonQuery() == 0)
            throw new Exception("Cuenta no encontrada: " + cuenta);
    }

    public Movimiento RegistrarDeposito(string cuenta, decimal importe, string empleado = "9999")
    {
        using var cn = GetConnection();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            int numero = ObtenerSiguienteMovimiento(cn, tx, cuenta);
            ActualizarSaldo(cn, tx, cuenta, importe);
            using var cmd = new SqlCommand(@"INSERT INTO Movimiento(chr_cuencodigo,int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,dec_moviimporte,chr_cuenreferencia)
                                            VALUES(@cuenta,@num,GETDATE(),@empl,'003',@importe,NULL)", cn, tx);
            cmd.Parameters.AddWithValue("@cuenta", cuenta);
            cmd.Parameters.AddWithValue("@num", numero);
            cmd.Parameters.AddWithValue("@empl", empleado);
            cmd.Parameters.AddWithValue("@importe", importe);
            cmd.ExecuteNonQuery();
            tx.Commit();
            return new Movimiento { CuentaCodigo = cuenta, Numero = numero, Fecha = DateTime.Now, EmpleadoCodigo = empleado, TipoCodigo = "003", Importe = importe };
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public Movimiento RegistrarRetiro(string cuenta, decimal importe, string empleado = "9999")
    {
        using var cn = GetConnection();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            // Verificar saldo suficiente
            using (var cmdSaldo = new SqlCommand("SELECT dec_cuensaldo FROM Cuenta WHERE chr_cuencodigo=@c", cn, tx))
            {
                cmdSaldo.Parameters.AddWithValue("@c", cuenta);
                var saldo = (decimal?)cmdSaldo.ExecuteScalar();
                if (saldo == null) throw new Exception("Cuenta no encontrada");
                if (saldo < importe) throw new Exception("Saldo insuficiente");
            }
            int numero = ObtenerSiguienteMovimiento(cn, tx, cuenta);
            ActualizarSaldo(cn, tx, cuenta, -importe);
            using var cmd = new SqlCommand(@"INSERT INTO Movimiento(chr_cuencodigo,int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,dec_moviimporte,chr_cuenreferencia)
                                            VALUES(@cuenta,@num,GETDATE(),@empl,'004',@importe,NULL)", cn, tx);
            cmd.Parameters.AddWithValue("@cuenta", cuenta);
            cmd.Parameters.AddWithValue("@num", numero);
            cmd.Parameters.AddWithValue("@empl", empleado);
            cmd.Parameters.AddWithValue("@importe", importe);
            cmd.ExecuteNonQuery();
            tx.Commit();
            return new Movimiento { CuentaCodigo = cuenta, Numero = numero, Fecha = DateTime.Now, EmpleadoCodigo = empleado, TipoCodigo = "004", Importe = importe };
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public IEnumerable<Movimiento> RegistrarTransferencia(string origen, string destino, decimal importe, string empleado = "9999")
    {
        using var cn = GetConnection();
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            // Validar cuentas y saldo
            decimal? saldoOrigen;
            using (var cmd = new SqlCommand("SELECT dec_cuensaldo FROM Cuenta WHERE chr_cuencodigo=@c", cn, tx))
            {
                cmd.Parameters.AddWithValue("@c", origen);
                saldoOrigen = (decimal?)cmd.ExecuteScalar();
            }
            if (saldoOrigen == null) throw new Exception("Cuenta origen no existe");
            if (saldoOrigen < importe) throw new Exception("Saldo insuficiente en cuenta origen");
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM Cuenta WHERE chr_cuencodigo=@c", cn, tx))
            {
                cmd.Parameters.AddWithValue("@c", destino);
                if ((int)cmd.ExecuteScalar() == 0) throw new Exception("Cuenta destino no existe");
            }

            // Movimiento salida (009) en origen
            int numOrigen = ObtenerSiguienteMovimiento(cn, tx, origen);
            ActualizarSaldo(cn, tx, origen, -importe);
            using (var cmdOut = new SqlCommand(@"INSERT INTO Movimiento(chr_cuencodigo,int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,dec_moviimporte,chr_cuenreferencia)
                                                 VALUES(@c,@n,GETDATE(),@e,'009',@imp,@ref)", cn, tx))
            {
                cmdOut.Parameters.AddWithValue("@c", origen);
                cmdOut.Parameters.AddWithValue("@n", numOrigen);
                cmdOut.Parameters.AddWithValue("@e", empleado);
                cmdOut.Parameters.AddWithValue("@imp", importe);
                cmdOut.Parameters.AddWithValue("@ref", destino);
                cmdOut.ExecuteNonQuery();
            }
            // Movimiento ingreso (008) en destino
            int numDestino = ObtenerSiguienteMovimiento(cn, tx, destino);
            ActualizarSaldo(cn, tx, destino, importe);
            using (var cmdIn = new SqlCommand(@"INSERT INTO Movimiento(chr_cuencodigo,int_movinumero,dtt_movifecha,chr_emplcodigo,chr_tipocodigo,dec_moviimporte,chr_cuenreferencia)
                                                VALUES(@c,@n,GETDATE(),@e,'008',@imp,@ref)", cn, tx))
            {
                cmdIn.Parameters.AddWithValue("@c", destino);
                cmdIn.Parameters.AddWithValue("@n", numDestino);
                cmdIn.Parameters.AddWithValue("@e", empleado);
                cmdIn.Parameters.AddWithValue("@imp", importe);
                cmdIn.Parameters.AddWithValue("@ref", origen);
                cmdIn.ExecuteNonQuery();
            }

            tx.Commit();
            return new List<Movimiento>
            {
                new Movimiento { CuentaCodigo = origen, Numero = numOrigen, Fecha = DateTime.Now, EmpleadoCodigo = empleado, TipoCodigo = "009", Importe = importe, ReferenciaCuenta = destino },
                new Movimiento { CuentaCodigo = destino, Numero = numDestino, Fecha = DateTime.Now, EmpleadoCodigo = empleado, TipoCodigo = "008", Importe = importe, ReferenciaCuenta = origen }
            };
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}

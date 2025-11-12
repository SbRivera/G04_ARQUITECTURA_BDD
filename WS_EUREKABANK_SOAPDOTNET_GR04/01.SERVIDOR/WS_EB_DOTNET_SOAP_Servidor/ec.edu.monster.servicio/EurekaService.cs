using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.modelo;

namespace WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.db
{
    public class EurekaService
    {
        private readonly string connectionString;

        public EurekaService()
        {
            // 1️⃣ Si hay cadena en Web.config, se usa
            var cfg = ConfigurationManager.ConnectionStrings["EUREKABANK"]?.ConnectionString;
            if (!string.IsNullOrWhiteSpace(cfg) && TryOpen(cfg, out _))
            {
                connectionString = cfg;
                return;
            }

            // 2️⃣ Autodetección sin configuración previa
            var candidates = new[]
            {
                "Server=localhost;Database=EUREKABANK;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True",
                "Server=.;Database=EUREKABANK;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True",
                "Server=.\\SQLEXPRESS;Database=EUREKABANK;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True",
                "Server=(localdb)\\MSSQLLocalDB;Database=EUREKABANK;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
            };

            var errores = new List<string>();
            foreach (var cs in candidates)
            {
                if (TryOpen(cs, out var err))
                {
                    connectionString = cs;
                    return;
                }
                errores.Add(err);
            }

            throw new InvalidOperationException(
                "❌ No se encontró la base 'EUREKABANK'. " +
                "Crea/restaura la BD con ese nombre en alguna instancia local (localhost, .\\SQLEXPRESS o (localdb)\\MSSQLLocalDB) " +
                "y asegúrate de tener acceso con Windows Authentication.\n\n" +
                "Errores:\n- " + string.Join("\n- ", errores));
        }

        private static bool TryOpen(string cs, out string error)
        {
            try
            {
                using (var cn = new SqlConnection(cs))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand("SELECT DB_NAME()", cn))
                    {
                        var db = (string)cmd.ExecuteScalar();
                        if (!string.Equals(db, "EUREKABANK", StringComparison.OrdinalIgnoreCase))
                        {
                            error = $"Conectó pero no a EUREKABANK (DB_NAME()={db}).";
                            return false;
                        }
                    }
                }
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private const string USUARIO = "MONSTER";
        private static readonly string PASSWORD = CrearHash("MONSTER9");

        public string ProbarConexion()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT GETDATE() AS FechaServidor, @@VERSION AS VersionSQL, DB_NAME() AS BaseDatos";
                    var cmd = new SqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return $"✅ CONEXIÓN EXITOSA\n📅 Fecha servidor: {reader["FechaServidor"]}\n" +
                                   $"🗄️ Base: {reader["BaseDatos"]}\n" +
                                   $"🔧 SQL: {reader["VersionSQL"].ToString().Substring(0, 80)}...";
                        }
                    }
                }
                return "❌ No se pudo obtener información del servidor.";
            }
            catch (Exception ex)
            {
                return $"❌ ERROR DE CONEXIÓN: {ex.Message}\n\n🔧 String usado:\n{connectionString}";
            }
        }

        public string VerificarTablas()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT TABLE_NAME,
                               (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) AS NumColumnas
                        FROM INFORMATION_SCHEMA.TABLES t
                        WHERE TABLE_TYPE='BASE TABLE'
                          AND TABLE_NAME IN ('cuenta','movimiento','tipomovimiento')
                        ORDER BY TABLE_NAME";
                    var cmd = new SqlCommand(query, connection);
                    var sb = new StringBuilder("📋 VERIFICACIÓN DE TABLAS:\n\n");

                    using (var reader = cmd.ExecuteReader())
                    {
                        bool found = false;
                        while (reader.Read())
                        {
                            found = true;
                            sb.AppendLine($"✅ {reader["TABLE_NAME"]} ({reader["NumColumnas"]} columnas)");
                        }
                        if (!found)
                            sb.AppendLine("⚠️ No se encontraron las tablas esperadas.");
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"❌ ERROR AL VERIFICAR TABLAS: {ex.Message}";
            }
        }

        public bool ValidarIngreso(string usuario, string password)
        {
            return USUARIO.Equals(usuario) && PASSWORD.Equals(CrearHash(password));
        }

        public static string CrearHash(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        public List<Movimiento> LeerMovimientos(string cuenta)
        {
            var lista = new List<Movimiento>();
            using (var cn = new SqlConnection(connectionString))
            {
                var q = @"
                    SELECT m.chr_cuencodigo AS Cuenta,
                           m.int_movinumero AS NroMov,
                           m.dtt_movifecha AS Fecha,
                           t.vch_tipodescripcion AS Tipo,
                           t.vch_tipoaccion AS Accion,
                           m.dec_moviimporte AS Importe
                    FROM dbo.tipomovimiento t
                    JOIN dbo.movimiento m ON t.chr_tipocodigo = m.chr_tipocodigo
                    WHERE m.chr_cuencodigo = @Cuenta
                    ORDER BY m.int_movinumero";
                var cmd = new SqlCommand(q, cn);
                cmd.Parameters.AddWithValue("@Cuenta", cuenta);

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Movimiento
                        {
                            Cuenta = rd["Cuenta"].ToString(),
                            NroMov = Convert.ToInt32(rd["NroMov"]),
                            Fecha = Convert.ToDateTime(rd["Fecha"]),
                            Tipo = rd["Tipo"].ToString(),
                            Accion = rd["Accion"].ToString(),
                            Importe = Convert.ToDouble(rd["Importe"])
                        });
                    }
                }
            }
            return lista;
        }

        // 🔁 Transacciones atómicas
        public void RegistrarTransferencia(string origen, string destino, double importe)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        AfectarCuenta(cn, tx, origen, -importe, "004"); // Retiro
                        AfectarCuenta(cn, tx, destino, +importe, "003"); // Depósito
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private void AfectarCuenta(SqlConnection cn, SqlTransaction tx, string cuenta, double delta, string tipo)
        {
            var qSel = @"SELECT dec_cuensaldo, int_cuencontmov FROM dbo.cuenta WITH (UPDLOCK) WHERE chr_cuencodigo=@Cuenta AND vch_cuenestado='ACTIVO'";
            double saldo;
            int cont;

            using (var cmd = new SqlCommand(qSel, cn, tx))
            {
                cmd.Parameters.AddWithValue("@Cuenta", cuenta);
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        throw new Exception($"La cuenta {cuenta} no existe o no está activa.");
                    saldo = Convert.ToDouble(rd["dec_cuensaldo"]);
                    cont = Convert.ToInt32(rd["int_cuencontmov"]);
                }
            }

            if (delta < 0 && saldo < Math.Abs(delta))
                throw new Exception($"Saldo insuficiente en cuenta {cuenta}.");

            saldo += delta;
            cont++;

            var qUpd = "UPDATE dbo.cuenta SET dec_cuensaldo=@s, int_cuencontmov=@c WHERE chr_cuencodigo=@Cuenta";
            using (var cmd = new SqlCommand(qUpd, cn, tx))
            {
                cmd.Parameters.AddWithValue("@s", saldo);
                cmd.Parameters.AddWithValue("@c", cont);
                cmd.Parameters.AddWithValue("@Cuenta", cuenta);
                cmd.ExecuteNonQuery();
            }

            var qIns = @"INSERT INTO dbo.movimiento (chr_cuencodigo, int_movinumero, dtt_movifecha, chr_emplcodigo, chr_tipocodigo, dec_moviimporte)
                         VALUES (@Cuenta, @nro, GETDATE(), '0001', @Tipo, @Importe)";
            using (var cmd = new SqlCommand(qIns, cn, tx))
            {
                cmd.Parameters.AddWithValue("@Cuenta", cuenta);
                cmd.Parameters.AddWithValue("@nro", cont);
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@Importe", delta);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

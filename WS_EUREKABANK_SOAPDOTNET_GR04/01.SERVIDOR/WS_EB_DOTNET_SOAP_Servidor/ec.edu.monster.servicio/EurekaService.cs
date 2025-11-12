using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            // 1) Desde Web.config si existe y funciona
            var cfg = ConfigurationManager.ConnectionStrings["EUREKABANK"]?.ConnectionString;
            if (!string.IsNullOrWhiteSpace(cfg) && TryOpen(cfg, out _))
            {
                connectionString = cfg;
                return;
            }

            // 2) Autodetección local
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
                "No se encontró una instancia de SQL Server con la base 'EUREKABANK'. " +
                "Crea/restaura la BD con ese nombre en localhost, .\\SQLEXPRESS o (localdb)\\MSSQLLocalDB y con acceso por Windows.\n\n" +
                "Intentos fallidos:\n- " + string.Join("\n- ", errores));
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

        // ====== Login ======
        private const string USUARIO = "MONSTER";
        private static readonly string PASSWORD = CrearHash("MONSTER9");

        public bool ValidarIngreso(string usuario, string password)
            => USUARIO.Equals(usuario) && PASSWORD.Equals(CrearHash(password));

        public static string CrearHash(string password)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        // ====== Lectura de movimientos ======
        public List<Movimiento> LeerMovimientos(string cuenta)
        {
            var lista = new List<Movimiento>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(@"
                    SELECT 
                        m.chr_cuencodigo   AS Cuenta,
                        m.int_movinumero   AS NroMov,
                        m.dtt_movifecha    AS Fecha,
                        t.vch_tipodescripcion AS Tipo,
                        t.vch_tipoaccion   AS Accion,
                        m.dec_moviimporte  AS Importe
                    FROM dbo.tipomovimiento t
                    INNER JOIN dbo.movimiento m ON t.chr_tipocodigo = m.chr_tipocodigo
                    WHERE m.chr_cuencodigo = @Cuenta
                    ORDER BY m.dtt_movifecha DESC, m.int_movinumero DESC", cn))
            {
                cmd.Parameters.Add(new SqlParameter("@Cuenta", SqlDbType.Char, 8) { Value = cuenta });

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    int iImp = rd.GetOrdinal("Importe");
                    while (rd.Read())
                    {
                        lista.Add(new Movimiento
                        {
                            Cuenta = rd["Cuenta"].ToString(),
                            NroMov = Convert.ToInt32(rd["NroMov"]),
                            Fecha = Convert.ToDateTime(rd["Fecha"]),
                            Tipo = rd["Tipo"].ToString(),
                            Accion = rd["Accion"].ToString(),
                            Importe = rd.IsDBNull(iImp) ? 0m : rd.GetDecimal(iImp)
                        });
                    }
                }
            }
            return lista;
        }

        // ====== Operaciones (devuelven saldo) ======
        public decimal RegistrarDeposito(string cuenta, decimal importe, string codEmp)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        var saldo = AfectarCuentaYRetornarSaldo(cn, tx, cuenta, +importe, codEmp, "003");
                        tx.Commit();
                        return saldo;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public decimal RegistrarRetiro(string cuenta, decimal importe, string codEmp)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        var saldo = AfectarCuentaYRetornarSaldo(cn, tx, cuenta, -importe, codEmp, "004");
                        tx.Commit();
                        return saldo;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // Retorna el saldo actualizado de la cuenta ORIGEN (como en Java)
        public decimal RegistrarTransferencia(string cuentaOrigen, string cuentaDestino, decimal importe, string codEmp)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // Orden estable para evitar deadlocks
                        var a = string.CompareOrdinal(cuentaOrigen, cuentaDestino) <= 0 ? cuentaOrigen : cuentaDestino;
                        var b = a == cuentaOrigen ? cuentaDestino : cuentaOrigen;

                        PrelockCuenta(cn, tx, a);
                        PrelockCuenta(cn, tx, b);

                        var saldoOrigen = AfectarCuentaYRetornarSaldo(cn, tx, cuentaOrigen, -importe, codEmp, "009");
                        _ = AfectarCuentaYRetornarSaldo(cn, tx, cuentaDestino, +importe, codEmp, "008");

                        tx.Commit();
                        return saldoOrigen;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private void PrelockCuenta(SqlConnection cn, SqlTransaction tx, string cuenta)
        {
            var q = @"
        SELECT 1
        FROM dbo.cuenta WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
        WHERE chr_cuencodigo=@Cuenta AND vch_cuenestado='ACTIVO';";

            using (var cmd = new SqlCommand(q, cn, tx))
            {
                cmd.Parameters.Add(new SqlParameter("@Cuenta", System.Data.SqlDbType.Char, 8) { Value = cuenta });
                var ok = cmd.ExecuteScalar();
                if (ok == null) throw new Exception($"La cuenta {cuenta} no existe o no está activa.");
            }
        }


        // ====== Core común ======
        private decimal AfectarCuentaYRetornarSaldo(SqlConnection cn, SqlTransaction tx, string cuenta, decimal delta, string codEmp, string tipoCodigo)
        {
            // SELECT con bloqueo equivalente a FOR UPDATE
            var qSel = @"
                SELECT dec_cuensaldo, int_cuencontmov
                FROM dbo.cuenta WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
                WHERE chr_cuencodigo=@Cuenta AND vch_cuenestado='ACTIVO';";

            decimal saldo;
            int cont;

            using (var cmd = new SqlCommand(qSel, cn, tx))
            {
                cmd.Parameters.Add(new SqlParameter("@Cuenta", SqlDbType.Char, 8) { Value = cuenta });
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        throw new Exception($"La cuenta {cuenta} no existe o no está activa.");

                    saldo = Convert.ToDecimal(rd["dec_cuensaldo"]);
                    cont = Convert.ToInt32(rd["int_cuencontmov"]);
                }
            }

            if (delta < 0 && saldo < Math.Abs(delta))
                throw new Exception("Saldo insuficiente.");

            saldo += delta;
            cont++;

            var qUpd = @"
                UPDATE dbo.cuenta 
                SET dec_cuensaldo=@s, int_cuencontmov=@c
                WHERE chr_cuencodigo=@Cuenta AND vch_cuenestado='ACTIVO';";

            using (var cmd = new SqlCommand(qUpd, cn, tx))
            {
                var pS = new SqlParameter("@s", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = saldo };
                var pC = new SqlParameter("@c", SqlDbType.Int) { Value = cont };
                var pCuenta = new SqlParameter("@Cuenta", SqlDbType.Char, 8) { Value = cuenta };

                cmd.Parameters.Add(pS);
                cmd.Parameters.Add(pC);
                cmd.Parameters.Add(pCuenta);
                cmd.ExecuteNonQuery();
            }

            // Insertar importe POSITIVO (compatibilidad con Java)
            var qIns = @"
                INSERT INTO dbo.movimiento 
                  (chr_cuencodigo, int_movinumero, dtt_movifecha, chr_emplcodigo, chr_tipocodigo, dec_moviimporte)
                VALUES
                  (@Cuenta, @nro, GETDATE(), @Emp, @Tipo, @Importe);";

            using (var cmd = new SqlCommand(qIns, cn, tx))
            {
                var pCuenta = new SqlParameter("@Cuenta", SqlDbType.Char, 8) { Value = cuenta };
                var pNro = new SqlParameter("@nro", SqlDbType.Int) { Value = cont };
                var pEmp = new SqlParameter("@Emp", SqlDbType.Char, 4) { Value = string.IsNullOrWhiteSpace(codEmp) ? "0001" : codEmp };
                var pTipo = new SqlParameter("@Tipo", SqlDbType.Char, 3) { Value = tipoCodigo };
                var pImp = new SqlParameter("@Importe", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = Math.Abs(delta) };

                cmd.Parameters.Add(pCuenta);
                cmd.Parameters.Add(pNro);
                cmd.Parameters.Add(pEmp);
                cmd.Parameters.Add(pTipo);
                cmd.Parameters.Add(pImp);
                cmd.ExecuteNonQuery();
            }

            return Math.Round(saldo, 2);
        }

        // ====== Utilidades ======
        public string ProbarConexion()
        {
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    var cmd = new SqlCommand("SELECT GETDATE() AS FechaServidor, @@VERSION AS VersionSQL, DB_NAME() AS BaseDatos", cn);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                            return $"✅ CONEXIÓN OK - BD: {rd["BaseDatos"]} - {rd["VersionSQL"].ToString().Substring(0, 80)}...";
                    }
                }
                return "❌ No se pudo obtener info del servidor.";
            }
            catch (Exception ex)
            {
                return $"❌ Error de conexión: {ex.Message}";
            }
        }

        public string VerificarTablas()
        {
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    var q = @"
                        SELECT TABLE_NAME
                        FROM INFORMATION_SCHEMA.TABLES
                        WHERE TABLE_TYPE='BASE TABLE'
                          AND TABLE_NAME IN ('cuenta','movimiento','tipomovimiento')
                        ORDER BY TABLE_NAME;";
                    var cmd = new SqlCommand(q, cn);
                    var sb = new StringBuilder("📋 Tablas base:\n");
                    using (var rd = cmd.ExecuteReader())
                    {
                        bool ok = false;
                        while (rd.Read()) { ok = true; sb.AppendLine(" - " + rd["TABLE_NAME"]); }
                        if (!ok) sb.AppendLine("⚠️ No se hallaron tablas requeridas.");
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error verificando tablas: {ex.Message}";
            }
        }
    }
}

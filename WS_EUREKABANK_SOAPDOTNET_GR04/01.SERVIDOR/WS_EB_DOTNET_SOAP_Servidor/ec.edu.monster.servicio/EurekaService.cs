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
            connectionString = ConfigurationManager.ConnectionStrings["EUREKABANK"].ConnectionString;
        }

        private const string USUARIO = "MONSTER";
        private static readonly string PASSWORD = CrearHash("MONSTER9");

        /// <summary>
        /// Método para probar la conexión a la base de datos
        /// </summary>
        /// <returns>Mensaje indicando el estado de la conexión</returns>
        public string ProbarConexion()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Ejecutar una consulta simple para verificar la conexión
                    var query = "SELECT GETDATE() AS FechaServidor, @@VERSION AS VersionSQL, DB_NAME() AS BaseDatos";
                    var command = new SqlCommand(query, connection);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var fechaServidor = reader["FechaServidor"];
                            var versionSQL = reader["VersionSQL"];
                            var baseDatos = reader["BaseDatos"];

                            return $"✅ CONEXIÓN EXITOSA\n" +
                                   $"📅 Fecha del servidor: {fechaServidor}\n" +
                                   $"🗄️ Base de datos: {baseDatos}\n" +
                                   $"🔧 Versión SQL Server: {versionSQL.ToString().Substring(0, 100)}...";
                        }
                    }
                }
                return "❌ Error: No se pudo obtener información del servidor";
            }
            catch (Exception ex)
            {
                return $"❌ ERROR DE CONEXIÓN:\n{ex.Message}\n\n🔧 String de conexión usado:\n{connectionString}";
            }
        }

        /// <summary>
        /// Método para verificar si las tablas necesarias existen
        /// </summary>
        /// <returns>Estado de las tablas en la base de datos</returns>
        public string VerificarTablas()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT 
                            TABLE_NAME,
                            (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) AS NumColumnas
                        FROM INFORMATION_SCHEMA.TABLES t
                        WHERE TABLE_TYPE = 'BASE TABLE' 
                            AND TABLE_NAME IN ('cuenta', 'movimiento', 'tipomovimiento')
                        ORDER BY TABLE_NAME";

                    var command = new SqlCommand(query, connection);
                    var resultado = "📋 VERIFICACIÓN DE TABLAS:\n\n";

                    using (var reader = command.ExecuteReader())
                    {
                        bool tablasEncontradas = false;
                        while (reader.Read())
                        {
                            tablasEncontradas = true;
                            var nombreTabla = reader["TABLE_NAME"].ToString();
                            var numColumnas = reader["NumColumnas"].ToString();
                            resultado += $"✅ Tabla '{nombreTabla}' - {numColumnas} columnas\n";
                        }

                        if (!tablasEncontradas)
                        {
                            resultado += "⚠️ No se encontraron las tablas necesarias (cuenta, movimiento, tipomovimiento)";
                        }
                    }

                    return resultado;
                }
            }
            catch (Exception ex)
            {
                return $"❌ ERROR AL VERIFICAR TABLAS:\n{ex.Message}";
            }
        }

        public bool ValidarIngreso(string usuario, string password)
        {
            string hashIngresado = CrearHash(password);
            return USUARIO.Equals(usuario) && PASSWORD.Equals(hashIngresado);
        }

        public static string CrearHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public List<Movimiento> LeerMovimientos(string cuenta)
        {
            var movimientos = new List<Movimiento>();
            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"
                    SELECT 
                        m.chr_cuencodigo AS Cuenta,
                        m.int_movinumero AS NroMov,
                        m.dtt_movifecha AS Fecha,
                        t.vch_tipodescripcion AS Tipo,
                        t.vch_tipoaccion AS Accion,
                        m.dec_moviimporte AS Importe
                    FROM tipomovimiento t
                    INNER JOIN movimiento m ON t.chr_tipocodigo = m.chr_tipocodigo
                    WHERE m.chr_cuencodigo = @Cuenta
                    ORDER BY m.int_movinumero";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Cuenta", cuenta);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        movimientos.Add(new Movimiento
                        {
                            Cuenta = reader["Cuenta"].ToString(),
                            NroMov = Convert.ToInt32(reader["NroMov"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            Tipo = reader["Tipo"].ToString(),
                            Accion = reader["Accion"].ToString(),
                            Importe = Convert.ToDouble(reader["Importe"])
                        });
                    }
                }
            }
            return movimientos;
        }

        public void RegistrarDeposito(string cuenta, double importe)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var querySelect = @"
                    SELECT dec_cuensaldo, int_cuencontmov 
                    FROM cuenta WITH (UPDLOCK)
                    WHERE chr_cuencodigo = @Cuenta AND vch_cuenestado = 'ACTIVO'";
                        var selectCommand = new SqlCommand(querySelect, connection, transaction);
                        selectCommand.Parameters.AddWithValue("@Cuenta", cuenta);

                        double saldo;
                        int cont;

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new Exception("La cuenta no existe o no está activa.");

                            saldo = Convert.ToDouble(reader["dec_cuensaldo"]);
                            cont = Convert.ToInt32(reader["int_cuencontmov"]);
                        }

                        saldo += importe;
                        cont++;
                        var queryUpdate = @"
                    UPDATE cuenta
                    SET dec_cuensaldo = @Saldo, int_cuencontmov = @Cont
                    WHERE chr_cuencodigo = @Cuenta";
                        var updateCommand = new SqlCommand(queryUpdate, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@Saldo", saldo);
                        updateCommand.Parameters.AddWithValue("@Cont", cont);
                        updateCommand.Parameters.AddWithValue("@Cuenta", cuenta);
                        updateCommand.ExecuteNonQuery();

                        var queryInsert = @"
                    INSERT INTO movimiento (chr_cuencodigo, int_movinumero, dtt_movifecha, chr_emplcodigo, chr_tipocodigo, dec_moviimporte)
                    VALUES (@Cuenta, @NroMov, GETDATE(), '0001', '003', @Importe)";
                        var insertCommand = new SqlCommand(queryInsert, connection, transaction);
                        insertCommand.Parameters.AddWithValue("@Cuenta", cuenta);
                        insertCommand.Parameters.AddWithValue("@NroMov", cont);
                        insertCommand.Parameters.AddWithValue("@Importe", importe);
                        insertCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void RegistrarRetiro(string cuenta, double importe)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var querySelect = @"
                        SELECT dec_cuensaldo, int_cuencontmov 
                        FROM cuenta WITH (UPDLOCK)
                        WHERE chr_cuencodigo = @Cuenta AND vch_cuenestado = 'ACTIVO'";
                        var selectCommand = new SqlCommand(querySelect, connection, transaction);
                        selectCommand.Parameters.AddWithValue("@Cuenta", cuenta);

                        double saldo;
                        int cont;

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new Exception("La cuenta no existe o no está activa.");

                            saldo = Convert.ToDouble(reader["dec_cuensaldo"]);
                            cont = Convert.ToInt32(reader["int_cuencontmov"]);
                        }

                        if (saldo < importe)
                            throw new Exception("Saldo insuficiente.");

                        saldo -= importe;
                        cont++;
                        var queryUpdate = @"
                        UPDATE cuenta
                        SET dec_cuensaldo = @Saldo, int_cuencontmov = @Cont
                        WHERE chr_cuencodigo = @Cuenta";
                        var updateCommand = new SqlCommand(queryUpdate, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@Saldo", saldo);
                        updateCommand.Parameters.AddWithValue("@Cont", cont);
                        updateCommand.Parameters.AddWithValue("@Cuenta", cuenta);
                        updateCommand.ExecuteNonQuery();

                        var queryInsert = @"
                        INSERT INTO movimiento (chr_cuencodigo, int_movinumero, dtt_movifecha, chr_emplcodigo, chr_tipocodigo, dec_moviimporte)
                        VALUES (@Cuenta, @NroMov, GETDATE(), '0001', '004', @Importe)";
                        var insertCommand = new SqlCommand(queryInsert, connection, transaction);
                        insertCommand.Parameters.AddWithValue("@Cuenta", cuenta);
                        insertCommand.Parameters.AddWithValue("@NroMov", cont);
                        insertCommand.Parameters.AddWithValue("@Importe", -importe);
                        insertCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void RegistrarTransferencia(string cuentaOrigen, string cuentaDestino, double importe)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        RegistrarRetiro(cuentaOrigen, importe);
                        RegistrarDeposito(cuentaDestino, importe);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

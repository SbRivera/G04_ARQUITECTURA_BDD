using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PruebaConexion
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== PRUEBA DE CONEXIÓN A BASE DE DATOS ===\n");
            
 // Simular la cadena de conexión del Web.config
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=EUREKABANK;Integrated Security=True;TrustServerCertificate=True";
            
            Console.WriteLine($"Cadena de conexión: {connectionString}\n");
   
      try
      {
  using (var connection = new SqlConnection(connectionString))
      {
     Console.WriteLine("Intentando conectar...");
             connection.Open();
      Console.WriteLine("? CONEXIÓN EXITOSA!\n");
   
          // Probar consulta básica
         var query = "SELECT GETDATE() AS FechaServidor, DB_NAME() AS BaseDatos, @@VERSION AS Version";
      var command = new SqlCommand(query, connection);
     
   using (var reader = command.ExecuteReader())
        {
             if (reader.Read())
       {
        Console.WriteLine($"?? Fecha del servidor: {reader["FechaServidor"]}");
    Console.WriteLine($"??? Base de datos: {reader["BaseDatos"]}");
         Console.WriteLine($"?? Versión SQL Server: {reader["Version"].ToString().Substring(0, 80)}...\n");
       }
     }
                }
 
                // Probar consulta a tablas específicas
          Console.WriteLine("Verificando tablas...");
  using (var connection = new SqlConnection(connectionString))
    {
     connection.Open();
          var query = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
  WHERE TABLE_TYPE = 'BASE TABLE' 
    AND TABLE_NAME IN ('cuenta', 'movimiento', 'tipomovimiento')
    ORDER BY TABLE_NAME";
           var command = new SqlCommand(query, connection);
             
     using (var reader = command.ExecuteReader())
    {
       bool encontradas = false;
            while (reader.Read())
         {
      encontradas = true;
               Console.WriteLine($"? Tabla encontrada: {reader["TABLE_NAME"]}");
    }
            
           if (!encontradas)
            {
         Console.WriteLine("?? No se encontraron las tablas esperadas (cuenta, movimiento, tipomovimiento)");
 }
         }
        }
      
  Console.WriteLine("\n?? PRUEBA COMPLETADA EXITOSAMENTE!");
 }
       catch (Exception ex)
            {
            Console.WriteLine($"? ERROR DE CONEXIÓN:");
            Console.WriteLine($"Mensaje: {ex.Message}");
 Console.WriteLine($"Tipo: {ex.GetType().Name}\n");
     
      if (ex.InnerException != null)
     {
                  Console.WriteLine($"Error interno: {ex.InnerException.Message}");
    }
         }
         
            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
}
    }
}
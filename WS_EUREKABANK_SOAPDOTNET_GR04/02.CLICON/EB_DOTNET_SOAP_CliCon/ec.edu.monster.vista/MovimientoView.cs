using EB_DOTNET_SOAP_CliCon.ServicioEB;
using System;
using System.Collections.Generic;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista
{
    public class MovimientoView
    {
        public static void MostrarMovimientos(List<movimiento> movimientos)
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine("{0,-15} {1,-10} {2,-20} {3,-12} {4,-10} {5,12}",
                "Cuenta", "Nro Mov.", "Fecha", "Tipo", "Acción", "Importe");
            Console.WriteLine("================================================================================");

            foreach (var m in movimientos)
            {
                Console.WriteLine("{0,-15} {1,-10} {2,-20:yyyy-MM-dd HH:mm} {3,-12} {4,-10} {5,12:N2}",
                    m.cuenta, m.nromov, m.fecha, m.tipo, m.accion, m.importe);
            }
        }
    }
}

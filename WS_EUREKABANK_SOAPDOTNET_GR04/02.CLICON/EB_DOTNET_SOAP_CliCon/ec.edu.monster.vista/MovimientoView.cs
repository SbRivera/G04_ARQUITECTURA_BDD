using EB_DOTNET_SOAP_CliCon.ServicioEB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista
{
    public class MovimientoView
    {
        public static void MostrarMovimientos(List<Movimiento> movimientos)
        {
            Console.WriteLine("\n================================================================================");
            Console.WriteLine("{0,-15} {1,-15} {2,-20} {3,-10} {4,-10} {5,-10}",
                "Cuenta", "Nro Mov.", "Fecha", "Tipo", "Acción", "Importe");
            Console.WriteLine("================================================================================");

            foreach (var movimiento in movimientos)
            {
                Console.WriteLine("{0,-15} {1,-15} {2,-20} {3,-10} {4,-10} {5,-10:C}",
                    movimiento.Cuenta, movimiento.NroMov, movimiento.Fecha, movimiento.Tipo, movimiento.Accion, movimiento.Importe);
            }
        }

        public void MostrarMensaje(string mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}

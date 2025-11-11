using EB_DOTNET_SOAP_CliCon.ec.edu.monster.controlador;
using EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EB_DOTNET_SOAP_CliCon
{
    public class CliCon_Vista
    {
        private const string USUARIO = "MONSTER";
        private const string PASS = "MONSTER9";
        static void Main(string[] args)
        {
            var scanner = new ConsoleInputHelper();
            var controlador = new CliCon_Controlador();


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"
            ╔══════════════════════════════════════════════════════════════════════════════╗
            ║                                                                              ║
            ║                              E U R E K A B A N K                             ║
            ║                                                                              ║
            ╚══════════════════════════════════════════════════════════════════════════════╝
            ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n--------------------");
            Console.WriteLine("     BIENVENIDO     ");
            Console.WriteLine("--------------------");
            Console.ResetColor();

            // Validación de usuario
            Console.Write("\nIngrese su usuario: ");
            string usuario = Console.ReadLine();
            Console.Write("Ingrese su contraseña: ");
            string password = Console.ReadLine();

            if (USUARIO.Equals(usuario) && PASS.Equals(password))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nAcceso exitoso");
                Console.ResetColor();

                int opcion;
                do
                {
                    // Mostrar el menú
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("\n==========================");
                    Console.WriteLine("       MENÚ PRINCIPAL       ");
                    Console.WriteLine("==========================\n");
                    Console.ResetColor();
                    Console.WriteLine("1. Consultar movimientos");
                    Console.WriteLine("2. Realizar depósito");
                    Console.WriteLine("3. Realizar retiro");
                    Console.WriteLine("4. Realizar transferencia");
                    Console.WriteLine("5. Salir");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\nSeleccione una opción: ");
                    Console.ResetColor();

                    opcion = scanner.ReadInt();

                    switch (opcion)
                    {
                        case 1:
                            // Consultar movimientos
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("\n============================");
                            Console.WriteLine("         MOVIMIENTOS        ");
                            Console.WriteLine("============================");
                            Console.Write("\nIngrese el número de cuenta: ");
                            Console.ResetColor();
                            string cuenta = Console.ReadLine();

                            var movimientos = controlador.ObtenerMovimientos(cuenta);
                            if (movimientos.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("No se encontraron movimientos para la cuenta ingresada.");
                                Console.ResetColor();
                            }
                            else
                            {
                                MovimientoView.MostrarMovimientos(movimientos);
                            }
                            break;

                        case 2:
                            // Realizar depósito
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n============================");
                            Console.WriteLine("         DEPÓSITO          ");
                            Console.WriteLine("============================");
                            Console.Write("\nIngrese el número de cuenta: ");
                            Console.ResetColor();
                            string cuentaDeposito = Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("Ingrese el importe a depositar: ");
                            Console.ResetColor();
                            double importe = scanner.ReadDouble();

                            try
                            {
                                controlador.RegistrarDeposito(cuentaDeposito, importe);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\nDepósito realizado exitosamente.");
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\nError al realizar el depósito: {ex.Message}");
                                Console.ResetColor();
                            }
                            break;

                        case 3:
                            // Realizar depósito
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("\n============================");
                            Console.WriteLine("         RETIRO          ");
                            Console.WriteLine("============================");
                            Console.Write("\nIngrese el número de cuenta: ");
                            Console.ResetColor();
                            string cuentaRetiro = Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("Ingrese el importe a retirar: ");
                            Console.ResetColor();
                            double importeRetiro = scanner.ReadDouble();

                            try
                            {
                                controlador.RegistrarRetiro(cuentaRetiro, importeRetiro);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\nRetiro realizado exitosamente.");
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\nError al realizar el retiro: {ex.Message}");
                                Console.ResetColor();
                            }
                            break;

                        case 4:
                            // Realizar depósito
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n============================");
                            Console.WriteLine("         TRANSFERENCIA          ");
                            Console.WriteLine("============================");
                            Console.Write("\nIngrese el número de cuenta origen: ");
                            Console.ResetColor();
                            string cuentaOrigen = Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("\nIngrese el número de cuenta destino: ");
                            Console.ResetColor();
                            string cuentaDestino = Console.ReadLine();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("\nIngrese el importe a retirar: ");
                            Console.ResetColor();
                            double importeTransferencia = scanner.ReadDouble();

                            try
                            {
                                controlador.RegistrarTransferencia(cuentaOrigen, cuentaDestino, importeTransferencia);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\nRetiro realizado exitosamente.");
                                Console.ResetColor();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\nError al realizar el retiro: {ex.Message}");
                                Console.ResetColor();
                            }
                            break;
                        case 5:
                            // Salir
                            Console.WriteLine("\nSaliendo del sistema. ¡Gracias!");
                            Console.WriteLine("Presione cualquier tecla para salir");
                            Console.ReadKey(); // Espera a que el usuario presione una tecla
                            return;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nOpción no válida. Intente nuevamente.");
                            Console.ResetColor();
                            break;
                    }
                } while (opcion != 5);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Acceso denegado");
                Console.ResetColor();
            }
        }
    }

    public class ConsoleInputHelper
    {
        public int ReadInt()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Entrada inválida. Por favor, ingrese un número entero.");
                Console.ResetColor();
            }
        }

        public double ReadDouble()
        {
            while (true)
            {
                if (double.TryParse(Console.ReadLine(), out double result))
                    return result;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Entrada inválida. Por favor, ingrese un número válido.");
                Console.ResetColor();
            }
        }
    }
}

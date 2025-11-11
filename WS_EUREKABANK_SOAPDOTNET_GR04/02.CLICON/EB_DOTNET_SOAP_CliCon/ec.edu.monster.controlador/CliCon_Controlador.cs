using EB_DOTNET_SOAP_CliCon.ServicioEB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.controlador
{
    public class CliCon_Controlador
    {
    private readonly ServicioEB.EurekaServiceSoapClient servicio;

        public CliCon_Controlador()
   {
          servicio = new ServicioEB.EurekaServiceSoapClient();
  }

  public List<Movimiento> ObtenerMovimientos(string cuenta)
        {
          var movimientos = new List<Movimiento>();
            try
            {
  // Llama al método del servicio web
 var datos = servicio.TraerMovimientos(cuenta);

    // Mapea los datos del servicio al modelo
         foreach (var dato in datos)
  {
         movimientos.Add(new Movimiento
        {
     Cuenta = dato.Cuenta,
     NroMov = dato.NroMov,
Fecha = dato.Fecha,
   Tipo = dato.Tipo,
        Accion = dato.Accion,
        Importe = dato.Importe
            });
                }
         }
 catch (TimeoutException ex)
         {
 Console.WriteLine($"Error de timeout al obtener movimientos: La operación tardó demasiado tiempo. Verifique la conexión de red y el servidor. Detalles: {ex.Message}");
 }
         catch (EndpointNotFoundException ex)
          {
    Console.WriteLine($"Error de conexión: No se pudo conectar al servicio web. Verifique que el servidor esté disponible. Detalles: {ex.Message}");
          }
            catch (CommunicationException ex)
    {
    Console.WriteLine($"Error de comunicación: {ex.Message}");
            }
  catch (Exception ex)
       {
  Console.WriteLine($"Error al obtener movimientos: {ex.Message}");
            }
     return movimientos;
        }

        public void RegistrarDeposito(string cuenta, double importe)
  {
     try
    {
     servicio.RegistrarDeposito(cuenta, importe);
          //Console.WriteLine("Depósito registrado correctamente.");
            }
            catch (TimeoutException ex)
    {
     throw new Exception($"Error de timeout al registrar depósito: La operación tardó demasiado tiempo. Detalles: {ex.Message}");
  }
            catch (EndpointNotFoundException ex)
     {
          throw new Exception($"Error de conexión: No se pudo conectar al servicio web. Detalles: {ex.Message}");
            }
    catch (CommunicationException ex)
  {
      throw new Exception($"Error de comunicación al registrar depósito: {ex.Message}");
            }
     catch (Exception ex)
            {
     throw new Exception($"Error al registrar depósito: {ex.Message}");
            }
        }

        public void RegistrarRetiro(string cuenta, double importe)
        {
      try
     {
    servicio.RegistrarRetiro(cuenta, importe);
         //Console.WriteLine(resultado); // Imprime el mensaje de éxito o error devuelto por el servicio
 }
      catch (TimeoutException ex)
         {
     throw new Exception($"Error de timeout al registrar retiro: La operación tardó demasiado tiempo. Detalles: {ex.Message}");
     }
            catch (EndpointNotFoundException ex)
{
             throw new Exception($"Error de conexión: No se pudo conectar al servicio web. Detalles: {ex.Message}");
      }
   catch (CommunicationException ex)
   {
      throw new Exception($"Error de comunicación al registrar retiro: {ex.Message}");
            }
 catch (Exception ex)
            {
     throw new Exception($"Error al registrar el retiro: {ex.Message}");
    }
        }

        public void RegistrarTransferencia(string cuentaOrigen, string cuentaDestino, double importe)
        {
            try
     {
       servicio.RegistrarTransferencia(cuentaOrigen, cuentaDestino, importe);
     //Console.WriteLine(resultado); // Imprime el mensaje de éxito o error devuelto por el servicio
    }
            catch (TimeoutException ex)
            {
          throw new Exception($"Error de timeout al registrar transferencia: La operación tardó demasiado tiempo. Detalles: {ex.Message}");
            }
        catch (EndpointNotFoundException ex)
  {
      throw new Exception($"Error de conexión: No se pudo conectar al servicio web. Detalles: {ex.Message}");
      }
   catch (CommunicationException ex)
  {
                throw new Exception($"Error de comunicación al registrar transferencia: {ex.Message}");
    }
            catch (Exception ex)
 {
       throw new Exception($"Error al registrar la transferencia: {ex.Message}");
        }
        }
    }
}

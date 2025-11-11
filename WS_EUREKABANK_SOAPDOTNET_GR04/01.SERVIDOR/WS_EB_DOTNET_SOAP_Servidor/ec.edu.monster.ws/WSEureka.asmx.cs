using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.db;
using WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.modelo;

namespace WS_EB_DOTNET_SOAP_Servidor
{
    /// <summary>
    /// Descripción breve de EurekaService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class EurekaService : System.Web.Services.WebService
    {

        private readonly ec.edu.monster.db.EurekaService dbAccess = new ec.edu.monster.db.EurekaService();

        [WebMethod]
        public string ValidarIngreso(string usuario, string password)
        {
            bool exitoso = dbAccess.ValidarIngreso(usuario, password);
            return exitoso ? "Exitoso" : "Denegado";
        }

        [WebMethod]
        public List<Movimiento> TraerMovimientos(string cuenta)
        {
            return dbAccess.LeerMovimientos(cuenta);
        }

        [WebMethod]
        public string RegistrarDeposito(string cuenta, double importe)
        {
            try
            {
                dbAccess.RegistrarDeposito(cuenta, importe);
                return "Deposito registrado exitosamente.";
            }
            catch (System.Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [WebMethod]
        public string RegistrarRetiro(string cuenta, double importe)
        {
            try
            {
                dbAccess.RegistrarRetiro(cuenta, importe);
                return "Retiro registrado exitosamente.";
            }
            catch (System.Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [WebMethod]
        public string RegistrarTransferencia(string cuentaOrigen, string cuentaDestino, double importe)
        {
            try
            {
                dbAccess.RegistrarTransferencia(cuentaOrigen, cuentaDestino, importe);
                return "Transferencia registrada exitosamente.";
            }
            catch (System.Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


    }
}

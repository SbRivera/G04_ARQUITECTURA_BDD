using System;
using System.Collections.Generic;
using System.Web.Services;
using WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.db;
using WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.modelo;

namespace WS_EB_DOTNET_SOAP_Servidor
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class EurekaService : WebService
    {
        private readonly ec.edu.monster.db.EurekaService db = new ec.edu.monster.db.EurekaService();

        [WebMethod] public string ProbarConexion() => db.ProbarConexion();
        [WebMethod] public string VerificarTablas() => db.VerificarTablas();

        [WebMethod]
        public string ValidarIngreso(string usuario, string password)
            => db.ValidarIngreso(usuario, password) ? "Exitoso" : "Denegado";

        [WebMethod]
        public List<Movimiento> TraerMovimientos(string cuenta)
            => db.LeerMovimientos(cuenta);

        [WebMethod]
        public string RegistrarDeposito(string cuenta, double importe)
        {
            try { db.RegistrarTransferencia(null, cuenta, importe); return "Depósito registrado."; }
            catch (Exception ex) { return $"Error: {ex.Message}"; }
        }

        [WebMethod]
        public string RegistrarRetiro(string cuenta, double importe)
        {
            try { db.RegistrarTransferencia(cuenta, null, importe); return "Retiro registrado."; }
            catch (Exception ex) { return $"Error: {ex.Message}"; }
        }

        [WebMethod]
        public string RegistrarTransferencia(string origen, string destino, double importe)
        {
            try { db.RegistrarTransferencia(origen, destino, importe); return "Transferencia registrada."; }
            catch (Exception ex) { return $"Error: {ex.Message}"; }
        }
    }
}

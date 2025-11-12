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
    public class WSEureka : WebService
    {
        private readonly EurekaService service = new EurekaService();

        // ===== LOGIN =====
        [WebMethod(Description = "Validar ingreso (igual que Java)")]
        public string validarIngreso(string usuario, string password)
            => service.ValidarIngreso(usuario, password) ? "Exitoso" : "Denegado";

        // ===== Movimientos =====
        [WebMethod(Description = "Trae los movimientos de una cuenta (igual que Java)")]
        public List<Movimiento> traerMovimientos(string cuenta)
        {
            try { return service.LeerMovimientos(cuenta); }
            catch { return new List<Movimiento>(); }
        }

        // ===== Depósito =====
        [WebMethod(Description = "Registrar depósito y devolver saldo (igual que Java)")]
        public OperacionCuentaResponse regDeposito(string cuenta, decimal importe)
        {
            var resp = new OperacionCuentaResponse();
            const string codEmp = "0001";
            try
            {
                var saldo = service.RegistrarDeposito(cuenta, importe, codEmp);
                resp.Estado = 1;
                resp.Saldo = saldo;
            }
            catch
            {
                resp.Estado = -1;
                resp.Saldo = -1;
            }
            return resp;
        }

        // ===== Retiro =====
        [WebMethod(Description = "Registrar retiro y devolver saldo (igual que Java)")]
        public OperacionCuentaResponse regRetiro(string cuenta, decimal importe)
        {
            var resp = new OperacionCuentaResponse();
            const string codEmp = "0004";
            try
            {
                var saldo = service.RegistrarRetiro(cuenta, importe, codEmp);
                resp.Estado = 1;
                resp.Saldo = saldo;
            }
            catch
            {
                resp.Estado = -1;
                resp.Saldo = -1;
            }
            return resp;
        }

        // ===== Transferencia =====
        [WebMethod(Description = "Registrar transferencia (devuelve saldo de cuenta origen)")]
        public OperacionCuentaResponse regTransferencia(string cuentaOrigen, string cuentaDestino, decimal importe)
        {
            var resp = new OperacionCuentaResponse();
            const string codEmp = "0004";
            try
            {
                var saldoOrigen = service.RegistrarTransferencia(cuentaOrigen, cuentaDestino, importe, codEmp);
                resp.Estado = 1;
                resp.Saldo = saldoOrigen;
            }
            catch
            {
                resp.Estado = -1;
                resp.Saldo = -1;
            }
            return resp;
        }

        // ===== Utilidades =====
        [WebMethod(Description = "Ping de conexión")]
        public string probarConexion() => service.ProbarConexion();

        [WebMethod(Description = "Verificar tablas base")]
        public string verificarTablas() => service.VerificarTablas();
    }
}

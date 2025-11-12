using EB_DOTNET_SOAP_CliCon.ServicioEB;
using System.Collections.Generic;
using System.Linq;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.controlador
{
    public class CliCon_Controlador
    {
        private readonly WSEurekaSoapClient servicio;

        public CliCon_Controlador()
        {
            // Fuerza el uso del endpoint "WSEurekaSoap" definido en App.config
            servicio = new WSEurekaSoapClient("WSEurekaSoap");
        }

        public string ValidarIngreso(string usuario, string password)
            => servicio.validarIngreso(usuario, password);

        // OJO: usa el tipo 'movimiento' del proxy
        public List<movimiento> ObtenerMovimientos(string cuenta)
        {
            var datos = servicio.traerMovimientos(cuenta);
            // Al regenerar, 'traerMovimientos' devuelve 'movimiento[]'
            return datos?.ToList() ?? new List<movimiento>();
        }

        public OperacionCuentaResponse RegistrarDeposito(string cuenta, decimal importe)
            => servicio.regDeposito(cuenta, importe);

        public OperacionCuentaResponse RegistrarRetiro(string cuenta, decimal importe)
            => servicio.regRetiro(cuenta, importe);

        public OperacionCuentaResponse RegistrarTransferencia(string cuentaOrigen, string cuentaDestino, decimal importe)
            => servicio.regTransferencia(cuentaOrigen, cuentaDestino, importe);
    }
}

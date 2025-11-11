using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.modelo
{
    public class Movimiento
    {
        public string Cuenta { get; set; }
        public int NroMov { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Accion { get; set; }
        public double Importe { get; set; }
    }
}
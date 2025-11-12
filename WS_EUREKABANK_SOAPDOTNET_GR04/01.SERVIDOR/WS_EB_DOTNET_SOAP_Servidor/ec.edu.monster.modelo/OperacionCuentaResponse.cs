using System;
using System.Xml.Serialization;

namespace WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.modelo
{
    [Serializable]
    [XmlRoot("OperacionCuentaResponse")]
    public class OperacionCuentaResponse
    {
        [XmlElement("estado")]
        public int Estado { get; set; }

        [XmlElement("saldo")]
        public decimal Saldo { get; set; } // dinero -> decimal
    }
}

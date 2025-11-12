using System;
using System.Xml.Serialization;

namespace WS_EB_DOTNET_SOAP_Servidor.ec.edu.monster.modelo
{
    [Serializable]
    [XmlType("movimiento")]
    public class Movimiento
    {
        [XmlElement("cuenta")]
        public string Cuenta { get; set; }

        [XmlElement("nromov")]
        public int NroMov { get; set; }

        [XmlElement("fecha", DataType = "dateTime")]
        public DateTime Fecha { get; set; }

        [XmlElement("tipo")]
        public string Tipo { get; set; }

        [XmlElement("accion")]
        public string Accion { get; set; }

        [XmlElement("importe")]
        public decimal Importe { get; set; } // dinero -> decimal
    }
}

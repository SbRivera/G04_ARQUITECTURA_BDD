using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using ec.edu.monster.models;
using Proxy = ServiceReference1; // alias del Connected Service (WCF/ASMX)

namespace ec.edu.monster.services
{
    /// <summary>
    /// Cliente SOAP que consume WSEureka.asmx (servidor .NET).
    /// Construye el binding según el esquema de la URL (http/https).
    /// </summary>
    public class SoapEurekaApi : IEurekaApi, IDisposable
    {
        private readonly Proxy.WSEurekaSoapClient _cli;

        /// <param name="baseUrl">
        /// URL completa al asmx, por ejemplo:
        ///   - "http://localhost:51640/ec.edu.monster.ws/WSEureka.asmx"
        ///   - "https://localhost:44396/ec.edu.monster.ws/WSEureka.asmx"
        /// Si es null, usa https://localhost:44396/...
        /// </param>
        /// <param name="trustLocalhostCertificate">
        /// Solo DEV: si true y la URL es https://localhost, omite la validación del certificado
        /// (evita “no se pudo establecer una relación de confianza…”).
        /// </param>
        public SoapEurekaApi(string baseUrl = null, bool trustLocalhostCertificate = true)
        {
            // Por defecto, la URL del IIS Express del servidor SOAP
            var url = string.IsNullOrWhiteSpace(baseUrl)
                ? "https://localhost:44396/ec.edu.monster.ws/WSEureka.asmx"
                : baseUrl.Trim();

            var address = new EndpointAddress(url);
            Binding binding;

            if (address.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                // HTTPS
                var b = new BasicHttpsBinding();
                // Si necesitas subir límites, descomenta:
                // b.MaxReceivedMessageSize = 10 * 1024 * 1024;
                // b.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                binding = b;

                // Forzar TLS 1.2 (común en entornos modernos)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Solo para DESARROLLO: confiar en certificado de localhost
                if (trustLocalhostCertificate &&
                    (address.Uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                     address.Uri.Host.Equals("127.0.0.1")))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                        (sender, cert, chain, errors) =>
                            errors == System.Net.Security.SslPolicyErrors.None ||
                            (cert != null && (cert.Subject?.Contains("CN=localhost") ?? false));
                }
            }
            else
            {
                // HTTP plano
                binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            }

            _cli = new Proxy.WSEurekaSoapClient(binding, address);

            System.Diagnostics.Debug.WriteLine("SOAP endpoint => " + _cli.Endpoint.Address.Uri);
        }

        /* ================= helpers de mapeo (reflexión) ================= */

        /// <summary>
        /// Lee una propiedad (case-insensitive) de un objeto proxy y la castea a T.
        /// Permite que nuestros modelos locales no dependan 1:1 de los tipos generados.
        /// </summary>
        private static T ReadProp<T>(object src, params string[] names)
        {
            if (src == null) return default!;
            var t = src.GetType();

            foreach (var n in names)
            {
                var p = t.GetProperty(n,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.IgnoreCase);

                if (p == null) continue;
                var v = p.GetValue(src);
                if (v == null) continue;

                try
                {
                    if (typeof(T) == typeof(decimal)) return (T)(object)Convert.ToDecimal(v);
                    if (typeof(T).IsEnum) return (T)Enum.Parse(typeof(T), v.ToString()!);
                    return (T)Convert.ChangeType(v, typeof(T));
                }
                catch
                {
                    return (T)v; // ya es del tipo adecuado
                }
            }

            return default!;
        }

        /* ================= implementación IEurekaApi ================= */

        public async Task<bool> ValidarIngreso(string usuario, string password)
        {
            // WCF Connected Service para ASMX suele generar: Task<string> validarIngresoAsync(...)
            var r = await _cli.validarIngresoAsync(usuario, password);
            return string.Equals(r, "Exitoso", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<Movimiento>> TraerMovimientos(string cuenta)
        {
            var list = new List<Movimiento>();

            // Suele ser movimiento[] o ArrayOfMovimiento; lo tratamos como IEnumerable
            var wsArray = await _cli.traerMovimientosAsync(cuenta);

            foreach (var m in (IEnumerable)wsArray)
            {
                list.Add(new Movimiento
                {
                    Cuenta = ReadProp<string>(m, "Cuenta", "cuenta"),
                    NroMov = ReadProp<int>(m, "NroMov", "nromov"),
                    Fecha = ReadProp<DateTime>(m, "Fecha", "fecha"),
                    Tipo = ReadProp<string>(m, "Tipo", "tipo"),
                    Accion = ReadProp<string>(m, "Accion", "accion"),
                    Importe = ReadProp<decimal>(m, "Importe", "importe")
                });
            }

            return list;
        }

        public async Task<OperacionCuentaResponse> RegDeposito(string cuenta, decimal importe)
        {
            var r = await _cli.regDepositoAsync(cuenta, importe);
            return new OperacionCuentaResponse
            {
                Estado = ReadProp<int>(r, "Estado", "estado"),
                Saldo = ReadProp<decimal>(r, "Saldo", "saldo")
            };
        }

        public async Task<OperacionCuentaResponse> RegRetiro(string cuenta, decimal importe)
        {
            var r = await _cli.regRetiroAsync(cuenta, importe);
            return new OperacionCuentaResponse
            {
                Estado = ReadProp<int>(r, "Estado", "estado"),
                Saldo = ReadProp<decimal>(r, "Saldo", "saldo")
            };
        }

        public async Task<OperacionCuentaResponse> RegTransferencia(string origen, string destino, decimal importe)
        {
            var r = await _cli.regTransferenciaAsync(origen, destino, importe);
            return new OperacionCuentaResponse
            {
                Estado = ReadProp<int>(r, "Estado", "estado"),
                Saldo = ReadProp<decimal>(r, "Saldo", "saldo")
            };
        }

        public async Task<string> ProbarConexion()
        {
            var r = await _cli.probarConexionAsync();
            return r;
        }

        public void Dispose()
        {
            try
            {
                if (_cli?.State == CommunicationState.Opened)
                    _cli.Close();
            }
            catch
            {
                _cli?.Abort();
            }
        }
    }
}

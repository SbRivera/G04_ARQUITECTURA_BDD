using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;   // AddServiceModelServices / UseServiceModel
using CoreWCF.Description;     // ServiceMetadataBehavior
using WS_EB_DOTNET_SOAP.Contracts;
using WS_EB_DOTNET_SOAP.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// DI de tu servicio de datos
builder.Services.AddSingleton<EurekaService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new EurekaService(config);
});

builder.Services.AddTransient<CoreBancarioSoap>();

// CoreWCF
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

var app = builder.Build();

// Ruta de prueba
app.MapGet("/ping", () => "pong");

// ====== ENDPOINTS ======
app.UseServiceModel(sb =>
{
    // Servicios
    sb.AddService<CoreBancarioSoap>();

    // ----- HTTPS (usa BasicHttpBinding con Security.Transport) -----
    var httpsBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
    sb.AddServiceEndpoint<CoreBancarioSoap, ICoreBancarioSoap>(httpsBinding, "/CoreBancario.svc");

    // (Opcional) HTTP en paralelo por si quieres probar sin TLS
    var httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
    sb.AddServiceEndpoint<CoreBancarioSoap, ICoreBancarioSoap>(httpBinding, "/soap/CoreBancario.svc");

    // WSDL
    var smb = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    smb.HttpGetEnabled = true;   // WSDL por HTTP (5070)
    smb.HttpsGetEnabled = true;  // WSDL por HTTPS (7299)
});

app.UseHttpsRedirection();
app.Run();

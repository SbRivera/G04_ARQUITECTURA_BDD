using Microsoft.AspNetCore.Mvc;
using WS_EB_DOTNET_REST_Servidor.Models;
using WS_EB_DOTNET_REST_Servidor.Services;

namespace WS_EB_DOTNET_REST_Servidor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoreBancarioController : ControllerBase
{
    private readonly EurekaService _service;
    public CoreBancarioController(EurekaService service)
    {
        _service = service;
    }

    [HttpGet("cuentas/{cuenta}/movimientos")]
    public ActionResult<IEnumerable<Movimiento>> GetMovimientos(string cuenta)
    {
        return Ok(_service.ListarMovimientos(cuenta));
    }

    public record OperacionRequest(string cuenta, decimal importe, string? empleado = null);
    public record TransferenciaRequest(string origen, string destino, decimal importe, string? empleado = null);

    [HttpPost("deposito")]
    public ActionResult<Movimiento> Deposito([FromBody] OperacionRequest req)
    {
        if (req.importe <= 0) return BadRequest("Importe inválido");
    var mov = _service.RegistrarDeposito(req.cuenta, req.importe, req.empleado ?? "9999");
        return Ok(mov);
    }

    [HttpPost("retiro")]
    public ActionResult<Movimiento> Retiro([FromBody] OperacionRequest req)
    {
        if (req.importe <= 0) return BadRequest("Importe inválido");
    var mov = _service.RegistrarRetiro(req.cuenta, req.importe, req.empleado ?? "9999");
        return Ok(mov);
    }

    [HttpPost("transferencia")]
    public ActionResult<IEnumerable<Movimiento>> Transferencia([FromBody] TransferenciaRequest req)
    {
        if (req.importe <= 0) return BadRequest("Importe inválido");
    var movs = _service.RegistrarTransferencia(req.origen, req.destino, req.importe, req.empleado ?? "9999");
        return Ok(movs);
    }
}

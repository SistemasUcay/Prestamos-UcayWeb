using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.Cobros.Commands;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrestamosUcayNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CobrosController : ControllerBase
{
    private readonly ICobrosService _cobrosService;

    public CobrosController(ICobrosService cobrosService)
    {
        _cobrosService = cobrosService;
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<List<AmortizacionPendienteDto>>> GetPendientes(
        [FromQuery] int? empresaId, 
        [FromQuery] int? tipoPagoId, 
        [FromQuery] DateTime? fechaVencimiento)
    {
        var result = await _cobrosService.GetAmortizacionesPendientesAsync(empresaId, tipoPagoId, fechaVencimiento);
        return Ok(result);
    }

    [HttpPost("aplicar-individual")]
    public async Task<IActionResult> AplicarPagoIndividual([FromBody] AplicarPagoIndividualCommand command)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            var result = await _cobrosService.AplicarPagoIndividualAsync(command, usuarioId);
            return Ok(new { Success = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ocurrió un error inesperado al procesar el pago.", Error = ex.Message });
        }
    }

    [HttpPost("aplicar-masivo")]
    public async Task<ActionResult<BulkPaymentResultDto>> AplicarPagoMasivo([FromBody] AplicarPagoMasivoCommand command)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            var result = await _cobrosService.AplicarPagoMasivoAsync(command, usuarioId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ocurrió un error inesperado al procesar la cobranza masiva.", Error = ex.Message });
        }
    }
}

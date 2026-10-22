using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.Cobros.Commands;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrestamosUcayNet.API.Controllers;

[Authorize(Roles = RolSistema.RolesOperacionPrestamos)]
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

    [HttpGet("prestamo/{prestamoId:int}/aplicados")]
    public async Task<ActionResult<List<CobroAplicadoDto>>> GetCobrosAplicados([FromRoute] int prestamoId)
    {
        try
        {
            var result = await _cobrosService.GetCobrosAplicadosPorPrestamoAsync(prestamoId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("cancelar/{cobroId:int}")]
    public async Task<IActionResult> CancelarCobro([FromRoute] int cobroId)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            var result = await _cobrosService.CancelarCobroAsync(cobroId, usuarioId);
            return Ok(new { Success = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ocurrió un error inesperado al cancelar el cobro.", Error = ex.Message });
        }
    }

    [HttpGet("lote/{loteId}/aplicados")]
    public async Task<ActionResult<List<CobroAplicadoDto>>> GetCobrosAplicadosPorLote([FromRoute] string loteId)
    {
        try
        {
            var result = await _cobrosService.GetCobrosAplicadosPorLoteAsync(loteId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("cancelar-lote/{loteId}")]
    public async Task<ActionResult<BulkCancelResultDto>> CancelarCobroMasivoPorLote([FromRoute] string loteId)
    {
        try
        {
            var usuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
            var result = await _cobrosService.CancelarCobroMasivoPorLoteAsync(loteId, usuarioId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Ocurrió un error inesperado al cancelar el lote de cobros.", Error = ex.Message });
        }
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

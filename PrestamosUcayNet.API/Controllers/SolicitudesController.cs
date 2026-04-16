using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrestamosUcayNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudesService _solicitudesService;

    public SolicitudesController(ISolicitudesService solicitudesService)
    {
        _solicitudesService = solicitudesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SolicitudDetalleDto>>> GetAll()
    {
        return Ok(await _solicitudesService.ListarTodasAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudDetalleDto>> Get(int id)
    {
        var result = await _solicitudesService.ObtenerPorIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<List<SolicitudDetalleDto>>> GetPendientes()
    {
        return Ok(await _solicitudesService.ListarPendientesAsync());
    }

    [HttpGet("aprobadas")]
    public async Task<ActionResult<List<SolicitudDetalleDto>>> GetAprobadas()
    {
        // Por simplicidad, filtramos aquí o creamos método en el servicio.
        // Vamos a filtrar en el servicio para mantener la arquitectura.
        return Ok(await _solicitudesService.ListarPorEstadoAsync("Aprobada"));
    }

    [HttpPost("evaluar-capacidad")]
    public async Task<ActionResult<CapacidadPagoResultDto>> EvaluarCapacidad(CapacidadPagoDto dto)
    {
        return Ok(await _solicitudesService.EvaluarCapacidadDePagoAsync(dto));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CrearSolicitudDto dto)
    {
        var id = await _solicitudesService.CrearSolicitudAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpPut]
    public async Task<IActionResult> Editar(EditarSolicitudDto dto)
    {
        var result = await _solicitudesService.EditarSolicitudAsync(dto);
        if (!result) return BadRequest("No se pudo editar la solicitud. Solo se pueden editar solicitudes en trámite.");
        return Ok();
    }

    [HttpPost("aprobar")]
    public async Task<IActionResult> Aprobar(AprobarSolicitudDto dto)
    {
        var result = await _solicitudesService.AprobarSolicitudAsync(dto);
        if (!result) return BadRequest("No se pudo aprobar la solicitud.");
        return Ok();
    }

    [HttpPost("rechazar")]
    public async Task<IActionResult> Rechazar(RechazarSolicitudDto dto)
    {
        var result = await _solicitudesService.RechazarSolicitudAsync(dto);
        if (!result) return BadRequest("No se pudo rechazar la solicitud.");
        return Ok();
    }

    [HttpPost("cancelar")]
    public async Task<IActionResult> Cancelar(CancelarSolicitudDto dto)
    {
        var result = await _solicitudesService.CancelarSolicitudAsync(dto);
        if (!result) return BadRequest("No se pudo cancelar la solicitud.");
        return Ok();
    }
}

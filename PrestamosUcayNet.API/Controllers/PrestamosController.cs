using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrestamosUcayNet.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly IPrestamosService _prestamosService;

    public PrestamosController(IPrestamosService prestamosService)
    {
        _prestamosService = prestamosService;
    }

    [HttpGet]
    [Authorize(Roles = RolSistema.RolesConsultaPrestamos)]
    public async Task<ActionResult<List<PrestamoListDto>>> GetAll()
    {
        return Ok(await _prestamosService.ListarTodosAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrestamoDetalleDto>> Get(int id)
    {
        var result = await _prestamosService.ObtenerDetalleAsync(id);
        if (result == null) return NotFound();

        // Si el usuario tiene únicamente el rol Empleado, validar que el préstamo le pertenezca
        if (User.IsInRole(RolSistema.Empleado) &&
            !User.IsInRole(RolSistema.Administrador) &&
            !User.IsInRole(RolSistema.Supervisor) &&
            !User.IsInRole(RolSistema.GestorPrestamos) &&
            !User.IsInRole(RolSistema.Consultor))
        {
            var claimEmpleadoId = User.FindFirst("empleadoId")?.Value;
            if (!int.TryParse(claimEmpleadoId, out var empId) || result.EmpleadoId != empId)
                return Forbid();
        }

        return Ok(result);
    }

    [HttpGet("por-estado/{estado}")]
    [Authorize(Roles = RolSistema.RolesConsultaPrestamos)]
    public async Task<ActionResult<List<PrestamoListDto>>> GetPorEstado(string estado)
    {
        return Ok(await _prestamosService.ListarPorEstadoAsync(estado));
    }

    [HttpGet("por-empleado/{empleadoId}")]
    public async Task<ActionResult<List<PrestamoListDto>>> GetPorEmpleado(int empleadoId)
    {
        // Si el usuario tiene únicamente el rol Empleado, solo puede consultar sus propios préstamos
        if (User.IsInRole(RolSistema.Empleado) &&
            !User.IsInRole(RolSistema.Administrador) &&
            !User.IsInRole(RolSistema.Supervisor) &&
            !User.IsInRole(RolSistema.GestorPrestamos) &&
            !User.IsInRole(RolSistema.Consultor))
        {
            var claimEmpleadoId = User.FindFirst("empleadoId")?.Value;
            if (!int.TryParse(claimEmpleadoId, out var empId) || empleadoId != empId)
                return Forbid();
        }

        return Ok(await _prestamosService.ListarPorEmpleadoAsync(empleadoId));
    }

    [HttpPost("activar")]
    [Authorize(Roles = RolSistema.RolesOperacionPrestamos)]
    public async Task<IActionResult> Activar(ActivarPrestamoDto dto)
    {
        try
        {
            var result = await _prestamosService.ActivarPrestamoAsync(dto);
            if (!result) return BadRequest("No se pudo activar el préstamo.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = RolSistema.RolesOperacionPrestamos)]
    public async Task<IActionResult> Crear(PrestamoCreateDto dto)
    {
        try
        {
            var prestamoId = await _prestamosService.CrearPrestamoAsync(dto);
            return Ok(new { prestamoId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("simular-amortizacion")]
    [Authorize(Roles = RolSistema.RolesOperacionPrestamos)]
    public ActionResult<AmortizacionPreviewResultDto> Simular(SimularAmortizacionDto dto)
    {
        var result = _prestamosService.SimularAmortizacion(dto);
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = RolSistema.RolesOperacionPrestamos)]
    public async Task<IActionResult> Actualizar(PrestamoUpdateDto dto)
    {
        var result = await _prestamosService.ActualizarPrestamoAsync(dto);
        if (!result) return NotFound("Préstamo no encontrado.");
        return Ok();
    }

    [HttpPost("{id}/cancelar")]
    [Authorize(Roles = RolSistema.RolesAprobadoresPrestamos)]
    public async Task<IActionResult> Cancelar(int id)
    {
        var result = await _prestamosService.CancelarPrestamoAsync(id);
        if (!result) return BadRequest("No se pudo cancelar el préstamo.");
        return Ok();
    }

    [HttpPost("evaluar")]
    [Authorize(Roles = RolSistema.RolesAprobadoresPrestamos)]
    public async Task<IActionResult> Evaluar(EvaluarPrestamoDto dto)
    {
        try
        {
            var result = await _prestamosService.EvaluarPrestamoAsync(dto);
            if (!result) return BadRequest("No se pudo evaluar el préstamo.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/aprobar")]
    [Authorize(Roles = RolSistema.RolesAprobadoresPrestamos)]
    public async Task<IActionResult> Aprobar(int id, [FromBody] AprobarPrestamoRequestDto request)
    {
        try
        {
            var result = await _prestamosService.AprobarPrestamoAsync(
                id,
                request.FechaEntrega,
                request.MetodoEntrega,
                request.Referencia,
                request.Folio,
                request.Notas);

            if (!result) return BadRequest("No se pudo aprobar el préstamo.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/rechazar")]
    [Authorize(Roles = RolSistema.RolesAprobadoresPrestamos)]
    public async Task<IActionResult> Rechazar(int id, [FromBody] RechazarCancelarRequestDto request)
    {
        try
        {
            var result = await _prestamosService.RechazarPrestamoAsync(id, request.Notas);
            if (!result) return BadRequest("No se pudo rechazar el préstamo.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/cancelar-solicitud")]
    [Authorize(Roles = RolSistema.RolesAprobadoresPrestamos)]
    public async Task<IActionResult> CancelarSolicitud(int id, [FromBody] RechazarCancelarRequestDto request)
    {
        try
        {
            var result = await _prestamosService.CancelarSolicitudAsync(id, request.Notas);
            if (!result) return BadRequest("No se pudo cancelar la solicitud.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/devolver-evaluacion")]
    [Authorize(Roles = RolSistema.RolesAprobadoresPrestamos)]
    public async Task<IActionResult> DevolverAEvaluacion(int id)
    {
        try
        {
            var result = await _prestamosService.DevolverAEvaluacionAsync(id);
            if (!result) return BadRequest("No se pudo devolver la solicitud a evaluación.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/solicitud")]
    [Authorize(Roles = RolSistema.RolesOperacionPrestamos)]
    public async Task<IActionResult> ActualizarSolicitud(int id, PrestamoCreateDto dto)
    {
        try
        {
            var result = await _prestamosService.ActualizarSolicitudAsync(id, dto);
            if (!result) return NotFound("Solicitud no encontrada.");
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class AprobarPrestamoRequestDto
{
    public DateTime FechaEntrega { get; set; } = DateTime.Now;
    public string MetodoEntrega { get; set; } = "Transferencia";
    public string Referencia { get; set; } = string.Empty;
    public string? Folio { get; set; }
    public string? Notas { get; set; }
}

public class RechazarCancelarRequestDto
{
    public string Notas { get; set; } = string.Empty;
}

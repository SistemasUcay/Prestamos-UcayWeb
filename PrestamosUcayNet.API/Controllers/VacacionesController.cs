using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.API.Controllers;

[Authorize(Roles = RolSistema.RolesAccesoVacaciones)]
[ApiController]
[Route("api/[controller]")]
public class VacacionesController : ControllerBase
{
    private readonly IVacacionesService _vacacionesService;

    public VacacionesController(IVacacionesService vacacionesService)
    {
        _vacacionesService = vacacionesService;
    }

    private bool EsSoloEmpleado(out int empleadoIdClaim)
    {
        empleadoIdClaim = 0;
        var esEmpleado = User.IsInRole(RolSistema.Empleado) &&
                         !User.IsInRole(RolSistema.Administrador) &&
                         !User.IsInRole(RolSistema.Supervisor) &&
                         !User.IsInRole(RolSistema.RecursosHumanos) &&
                         !User.IsInRole(RolSistema.Consultor);

        if (esEmpleado)
        {
            int.TryParse(User.FindFirst("empleadoId")?.Value, out empleadoIdClaim);
        }

        return esEmpleado;
    }

    [HttpGet("empleado/{empleadoId}/resumen")]
    public async Task<ActionResult<ResumenVacacionesEmpleadoDto>> ObtenerResumenPorEmpleado(
        int empleadoId,
        CancellationToken cancellationToken)
    {
        if (EsSoloEmpleado(out var empId) && empId != empleadoId)
            return Forbid();

        var resumen = await _vacacionesService.ObtenerResumenPorEmpleadoAsync(empleadoId, cancellationToken);
        if (resumen == null) return NotFound("Empleado no encontrado.");
        return Ok(resumen);
    }

    [HttpGet("empleado/{empleadoId}/periodos")]
    public async Task<ActionResult<IEnumerable<PeriodoVacacionalDto>>> ObtenerPeriodosPorEmpleado(
        int empleadoId,
        CancellationToken cancellationToken)
    {
        if (EsSoloEmpleado(out var empId) && empId != empleadoId)
            return Forbid();

        var periodos = await _vacacionesService.ObtenerPeriodosPorEmpleadoAsync(empleadoId, cancellationToken);
        return Ok(periodos);
    }

    [HttpGet("solicitudes")]
    public async Task<ActionResult<IEnumerable<SolicitudVacacionesListDto>>> ObtenerSolicitudes(
        [FromQuery] int? empleadoId,
        [FromQuery] int? empresaId,
        [FromQuery] string? estado,
        CancellationToken cancellationToken)
    {
        if (EsSoloEmpleado(out var empId))
        {
            if (empId <= 0) return Ok(new List<SolicitudVacacionesListDto>());
            empleadoId = empId;
        }

        var solicitudes = await _vacacionesService.ObtenerSolicitudesAsync(empleadoId, empresaId, estado, cancellationToken);
        return Ok(solicitudes);
    }

    [HttpGet("solicitudes/{id}")]
    public async Task<ActionResult<SolicitudVacacionesDetailDto>> ObtenerSolicitudPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var solicitud = await _vacacionesService.ObtenerSolicitudPorIdAsync(id, cancellationToken);
        if (solicitud == null) return NotFound("Solicitud de vacaciones no encontrada.");

        if (EsSoloEmpleado(out var empId) && solicitud.EmpleadoId != empId)
            return Forbid();

        return Ok(solicitud);
    }

    [HttpPost("solicitudes")]
    public async Task<ActionResult> CrearSolicitud(
        [FromBody] CrearSolicitudVacacionesDto dto,
        CancellationToken cancellationToken)
    {
        if (EsSoloEmpleado(out var empId) && dto.EmpleadoId != empId)
            return Forbid();

        var usuarioActual = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Usuario";
        var result = await _vacacionesService.CrearSolicitudAsync(dto, usuarioActual, cancellationToken);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(ObtenerSolicitudPorId), new { id = result.Id }, new { id = result.Id });
    }

    [HttpPut("solicitudes/{id}/resolver")]
    [Authorize(Roles = RolSistema.RolesAprobadoresVacaciones)]
    public async Task<ActionResult> ResolverSolicitud(
        int id,
        [FromBody] AprobarRechazarSolicitudDto dto,
        CancellationToken cancellationToken)
    {
        var usuarioActual = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Aprobador";
        var result = await _vacacionesService.ResolverSolicitudAsync(id, dto, usuarioActual, cancellationToken);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPut("solicitudes/{id}/cancelar")]
    [Authorize(Roles = RolSistema.RolesAprobadoresVacaciones)]
    public async Task<ActionResult> CancelarSolicitud(
        int id,
        CancellationToken cancellationToken)
    {
        var usuarioActual = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Usuario";
        var result = await _vacacionesService.CancelarSolicitudAsync(id, usuarioActual, cancellationToken);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }

    [HttpPost("empleado/{empleadoId}/generar-periodos")]
    [Authorize(Roles = RolSistema.RolesAprobadoresVacaciones)]
    public async Task<ActionResult> GenerarPeriodosEmpleado(
        int empleadoId,
        CancellationToken cancellationToken)
    {
        var usuarioActual = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Administrador";
        var result = await _vacacionesService.GenerarPeriodosEmpleadoAsync(empleadoId, usuarioActual, cancellationToken);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Ok(new { mensaje = "Periodos vacacionales generados y sincronizados correctamente." });
    }

    [HttpPost("aplicar-caducidad")]
    [Authorize(Roles = RolSistema.RolesAprobadoresVacaciones)]
    public async Task<ActionResult> AplicarCaducidad(
        [FromQuery] int? empleadoId,
        CancellationToken cancellationToken)
    {
        var usuarioActual = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Administrador";
        var result = await _vacacionesService.AplicarReglaCaducidadAnualAsync(empleadoId, usuarioActual, cancellationToken);

        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Ok(new { mensaje = $"Regla de caducidad anual aplicada exitosamente. {result.PeriodosAfectados} periodos actualizados." });
    }
}

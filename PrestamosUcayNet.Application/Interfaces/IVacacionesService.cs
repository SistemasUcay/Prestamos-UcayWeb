using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

/// <summary>
/// Contrato de servicio de aplicación para la gestión y control de vacaciones.
/// </summary>
public interface IVacacionesService
{
    /// <summary>
    /// Obtiene el resumen general de saldo, días y periodos de un empleado.
    /// </summary>
    Task<ResumenVacacionesEmpleadoDto?> ObtenerResumenPorEmpleadoAsync(int empleadoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los periodos vacacionales por año de un empleado.
    /// </summary>
    Task<IEnumerable<PeriodoVacacionalDto>> ObtenerPeriodosPorEmpleadoAsync(int empleadoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene solicitudes de vacaciones filtradas opcionalmente por empleado, empresa o estado.
    /// </summary>
    Task<IEnumerable<SolicitudVacacionesListDto>> ObtenerSolicitudesAsync(
        int? empleadoId = null,
        int? empresaId = null,
        string? estado = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el detalle de una solicitud específica por su ID.
    /// </summary>
    Task<SolicitudVacacionesDetailDto?> ObtenerSolicitudPorIdAsync(int solicitudId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra una nueva solicitud de vacaciones en estado Pendiente.
    /// </summary>
    Task<(bool Success, string? Error, int Id)> CrearSolicitudAsync(
        CrearSolicitudVacacionesDto dto,
        string usuarioCreacion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aprueba o rechaza una solicitud de vacaciones, actualizando los saldos del periodo vacacional si es aprobada.
    /// </summary>
    Task<(bool Success, string? Error)> ResolverSolicitudAsync(
        int solicitudId,
        AprobarRechazarSolicitudDto dto,
        string usuarioAprobador,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancela una solicitud de vacaciones (si ya estaba aprobada, restituye los días al periodo).
    /// </summary>
    Task<(bool Success, string? Error)> CancelarSolicitudAsync(
        int solicitudId,
        string usuario,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera o sincroniza los periodos anuales de vacaciones de un empleado con base en su fecha de ingreso y tabla legal.
    /// </summary>
    Task<(bool Success, string? Error)> GenerarPeriodosEmpleadoAsync(
        int empleadoId,
        string usuario,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aplica la regla de caducidad anual: si al año un periodo aún tiene días pendientes, se cancelan automáticamente,
    /// quedando siempre como saldo los días por disfrutar del año en curso.
    /// </summary>
    Task<(bool Success, string? Error, int PeriodosAfectados)> AplicarReglaCaducidadAnualAsync(
        int? empleadoId = null,
        string usuario = "Sistema",
        CancellationToken cancellationToken = default);
}

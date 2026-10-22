using System;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Representa el registro de solicitud y goce de días de vacaciones de un empleado asociado a un periodo vacacional.
/// </summary>
public partial class SolicitudVacaciones
{
    /// <summary>
    /// Identificador único de la solicitud de vacaciones.
    /// </summary>
    public int SolicitudVacacionesId { get; set; }

    /// <summary>
    /// Identificador del Empleado solicitante.
    /// </summary>
    public int EmpleadoId { get; set; }

    /// <summary>
    /// Identificador del Periodo Vacacional afectado.
    /// </summary>
    public int PeriodoVacacionalId { get; set; }

    /// <summary>
    /// Fecha en que se registra la solicitud.
    /// </summary>
    public DateTime FechaSolicitud { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de inicio del periodo vacacional solicitado.
    /// </summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Fecha de fin del periodo vacacional solicitado.
    /// </summary>
    public DateTime FechaFin { get; set; }

    /// <summary>
    /// Fecha programada de reincorporación laboral.
    /// </summary>
    public DateTime FechaRegreso { get; set; }

    /// <summary>
    /// Número total de días a disfrutar en esta solicitud.
    /// </summary>
    public int DiasSolicitados { get; set; }

    /// <summary>
    /// Estado actual de la solicitud: "Pendiente", "Aprobada", "Rechazada", "Cancelada".
    /// </summary>
    public string Estado { get; set; } = "Pendiente";

    /// <summary>
    /// Observaciones, motivos o justificación del empleado.
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Fecha en la que la solicitud fue resuelta (aprobada o rechazada).
    /// </summary>
    public DateTime? FechaAprobacion { get; set; }

    /// <summary>
    /// Usuario (o RH / Supervisor) que resolvió la solicitud.
    /// </summary>
    public string? UsuarioAprobador { get; set; }

    /// <summary>
    /// Comentarios u observaciones emitidas al autorizar o rechazar la solicitud.
    /// </summary>
    public string? ObservacionesAprobacion { get; set; }

    #region Auditoría Estándar
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public string UsuarioCreacion { get; set; } = null!;
    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    #endregion

    #region Relaciones de Navegación
    public virtual Empleado Empleado { get; set; } = null!;
    public virtual PeriodoVacacional PeriodoVacacional { get; set; } = null!;
    #endregion
}

using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Representa el periodo o saldo vacacional generado para un empleado por cada año de antigüedad.
/// </summary>
public partial class PeriodoVacacional
{
    /// <summary>
    /// Identificador único del Periodo Vacacional.
    /// </summary>
    public int PeriodoVacacionalId { get; set; }

    /// <summary>
    /// Identificador del Empleado titular.
    /// </summary>
    public int EmpleadoId { get; set; }

    /// <summary>
    /// Año correspondiente al ciclo laboral (ej. 2025, 2026).
    /// </summary>
    public int Anio { get; set; }

    /// <summary>
    /// Años de antigüedad o servicio cumplidos al momento del periodo.
    /// </summary>
    public int AntiguedadAnios { get; set; }

    /// <summary>
    /// Días de vacaciones que corresponden por ley o política interna en este ciclo.
    /// </summary>
    public int DiasCorrespondientes { get; set; }

    /// <summary>
    /// Total de días efectivamente disfrutados en este periodo.
    /// </summary>
    public int DiasDisfrutados { get; set; }

    /// <summary>
    /// Días restantes por disfrutar en este periodo (DiasCorrespondientes - DiasDisfrutados).
    /// </summary>
    public int DiasPendientes { get; set; }

    /// <summary>
    /// Fecha de inicio del ciclo anual del empleado.
    /// </summary>
    public DateTime FechaInicioPeriodo { get; set; }

    /// <summary>
    /// Fecha de culminación del ciclo anual del empleado.
    /// </summary>
    public DateTime FechaFinPeriodo { get; set; }

    /// <summary>
    /// Fecha límite legal o interna para disfrutar las vacaciones devengadas.
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }

    /// <summary>
    /// Estado del periodo: "Activo", "Cerrado", "Vencido".
    /// </summary>
    public string Estado { get; set; } = "Activo";

    #region Auditoría Estándar
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public string UsuarioCreacion { get; set; } = null!;
    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    #endregion

    #region Relaciones de Navegación
    public virtual Empleado Empleado { get; set; } = null!;
    public virtual ICollection<SolicitudVacaciones> SolicitudesVacaciones { get; set; } = new List<SolicitudVacaciones>();
    #endregion
}

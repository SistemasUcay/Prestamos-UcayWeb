using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Application.DTOs;

/// <summary>
/// DTO con la información de un periodo vacacional asignado a un empleado.
/// </summary>
public class PeriodoVacacionalDto
{
    public int PeriodoVacacionalId { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int AntiguedadAnios { get; set; }
    public int DiasCorrespondientes { get; set; }
    public int DiasDisfrutados { get; set; }
    public int DiasPendientes { get; set; }

    /// <summary>
    /// Fecha inicial del periodo que generó el derecho a vacaciones (año anterior).
    /// </summary>
    public DateTime FechaInicioPeriodo { get; set; }

    /// <summary>
    /// Fecha final del periodo que generó el derecho a vacaciones (año anterior).
    /// </summary>
    public DateTime FechaFinPeriodo { get; set; }

    /// <summary>
    /// Fecha límite del periodo de goce y disfrute (año en curso).
    /// </summary>
    public DateTime? FechaVencimiento { get; set; }

    /// <summary>
    /// Fecha inicial del periodo para disfrutar las vacaciones (día siguiente al término del periodo generador).
    /// </summary>
    public DateTime FechaInicioGoce => FechaFinPeriodo.Date.AddDays(1);

    /// <summary>
    /// Fecha final del periodo para disfrutar las vacaciones.
    /// </summary>
    public DateTime FechaFinGoce => FechaVencimiento?.Date ?? FechaFinPeriodo.Date.AddYears(1);

    /// <summary>
    /// Fecha exacta en que se cancelan automáticamente los días pendientes si no fueron disfrutados.
    /// </summary>
    public DateTime FechaCancelacion => FechaFinGoce.AddDays(1);

    public string PeriodoGeneradorTexto => $"{FechaInicioPeriodo:dd/MM/yyyy} al {FechaFinPeriodo:dd/MM/yyyy}";
    public string PeriodoGoceTexto => $"{FechaInicioGoce:dd/MM/yyyy} al {FechaFinGoce:dd/MM/yyyy}";

    public string Estado { get; set; } = string.Empty;
    public int DiasCancelados => Estado == "Vencido" ? Math.Max(0, DiasCorrespondientes - DiasDisfrutados - DiasPendientes) : 0;
}

/// <summary>
/// DTO para la visualización en listados y tablas de solicitudes de vacaciones.
/// </summary>
public class SolicitudVacacionesListDto
{
    public int SolicitudVacacionesId { get; set; }
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int NumeroNomina { get; set; }
    public string EmpresaNombre { get; set; } = string.Empty;
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string PuestoNombre { get; set; } = string.Empty;
    public DateTime? FechaIngresoEmpleado { get; set; }
    public int PeriodoVacacionalId { get; set; }
    public int AnioPeriodo { get; set; }
    public int AntiguedadAnios { get; set; }
    public DateTime? FechaInicioPeriodoGenerador { get; set; }
    public DateTime? FechaFinPeriodoGenerador { get; set; }
    public DateTime? FechaInicioPeriodoGoce => FechaFinPeriodoGenerador?.Date.AddDays(1);
    public DateTime? FechaFinPeriodoGoce { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaRegreso { get; set; }
    public int DiasSolicitados { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? UsuarioAprobador { get; set; }
}

/// <summary>
/// DTO con el detalle completo de una solicitud de vacaciones.
/// </summary>
public class SolicitudVacacionesDetailDto : SolicitudVacacionesListDto
{
    public string? ObservacionesAprobacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string UsuarioCreacion { get; set; } = string.Empty;
    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public int DiasCorrespondientesPeriodo { get; set; }
    public int DiasDisfrutadosPeriodo { get; set; }
    public int DiasPendientesPeriodo { get; set; }
}

/// <summary>
/// DTO para registrar una nueva solicitud de vacaciones.
/// </summary>
public class CrearSolicitudVacacionesDto
{
    public int EmpleadoId { get; set; }
    public int? PeriodoVacacionalId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaRegreso { get; set; }
    public int? DiasSolicitados { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para aprobar o rechazar una solicitud existente.
/// </summary>
public class AprobarRechazarSolicitudDto
{
    public bool Aprobada { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO con el resumen global de días de vacaciones y periodos para un empleado.
/// </summary>
public class ResumenVacacionesEmpleadoDto
{
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;
    public int NumeroNomina { get; set; }
    public string Rfc { get; set; } = string.Empty;
    public string EmpresaNombre { get; set; } = string.Empty;
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string PuestoNombre { get; set; } = string.Empty;
    public DateTime? FechaIngreso { get; set; }
    public int AntiguedadAnios { get; set; }
    public int DiasLeyPeriodoActual { get; set; }
    public int AnioPeriodoActual { get; set; }
    public DateTime? FechaInicioPeriodoGenerador { get; set; }
    public DateTime? FechaFinPeriodoGenerador { get; set; }
    public DateTime? FechaInicioPeriodoGoce { get; set; }
    public DateTime? FechaFinPeriodoGoce { get; set; }
    public DateTime? FechaCancelacionPeriodoActual { get; set; }
    public int TotalDiasDerecho { get; set; }
    public int TotalDiasDisfrutados { get; set; }
    public int TotalDiasPendientes { get; set; }
    public int TotalDiasCancelados { get; set; }
    public List<PeriodoVacacionalDto> Periodos { get; set; } = new();
    public List<SolicitudVacacionesListDto> SolicitudesRecientes { get; set; } = new();
}

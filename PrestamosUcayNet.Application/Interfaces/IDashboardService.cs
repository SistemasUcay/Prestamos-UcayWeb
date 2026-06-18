using System;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface IDashboardService
{
    /// <summary>
    /// Obtiene las estadísticas y métricas necesarias para el Dashboard principal.
    /// </summary>
    Task<DashboardDto> GetDashboardDataAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
}

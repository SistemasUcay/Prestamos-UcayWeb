using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class ParametrosGenerale
{
    /// <summary>
    /// Identificador de Parametros
    /// </summary>
    public int ParametrosGeneralesId { get; set; }

    /// <summary>
    /// Fecha Inicial del primer periodo semanal del Año para el sistema de Nómina
    /// </summary>
    public DateTime? InicioPeriodoSemanal { get; set; }

    /// <summary>
    /// Fecha Inicial del primer periodo catorcenal del Año para el sistema de Nómina
    /// </summary>
    public DateTime? InicioPeriodoCatorcenal { get; set; }

    /// <summary>
    /// Tasa Fija para Préstamos de Empresa
    /// </summary>
    public decimal? TasaFijaEmpresa { get; set; }

    /// <summary>
    /// Tasa Fija de Préstamos de la Caja de Ahorro
    /// </summary>
    public decimal? TasaFijaCaja { get; set; }

    /// <summary>
    /// Fecha de Cierre para la Caja de Ahorro
    /// </summary>
    public DateTime? CierreCajaAnual { get; set; }
}

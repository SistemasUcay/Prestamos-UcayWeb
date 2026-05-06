using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class ParametrosGenerale
{
    /// <summary>
    /// Año fiscal al que corresponden los parámetros (funciona como identificador único)
    /// </summary>
    public int Anio { get; set; }

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
    /// Tasa Fija para Préstamos Directos
    /// </summary>
    public decimal? TasaFijaDirecto { get; set; }

    /// <summary>
    /// Fecha de Cierre para la Caja de Ahorro
    /// </summary>
    public DateTime? CierreCajaAnual { get; set; }

    /// <summary>
    /// Fecha de Utilidades
    /// </summary>
    public DateTime? FechaUtilidades { get; set; }

    /// <summary>
    /// Fecha de Entrega de Aguinaldo
    /// </summary>
    public DateTime? FechaAguinaldo { get; set; }
}

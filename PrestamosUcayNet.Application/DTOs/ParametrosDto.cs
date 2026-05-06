using System;

namespace PrestamosUcayNet.Application.DTOs;

public class ParametrosDto
{
    public int Anio { get; set; }
    public DateTime? InicioPeriodoSemanal { get; set; }
    public DateTime? InicioPeriodoCatorcenal { get; set; }
    public decimal? TasaFijaEmpresa { get; set; }
    public decimal? TasaFijaCaja { get; set; }
    public decimal? TasaFijaDirecto { get; set; }
    public DateTime? CierreCajaAnual { get; set; }
    public DateTime? FechaUtilidades { get; set; }
    public DateTime? FechaAguinaldo { get; set; }
}

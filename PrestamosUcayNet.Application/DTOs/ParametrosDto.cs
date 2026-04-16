using System;

namespace PrestamosUcayNet.Application.DTOs;

public class ParametrosDto
{
    public int ParametrosGeneralesId { get; set; }
    public DateTime? InicioPeriodoSemanal { get; set; }
    public DateTime? InicioPeriodoCatorcenal { get; set; }
    public decimal? TasaFijaEmpresa { get; set; }
    public decimal? TasaFijaCaja { get; set; }
    public DateTime? CierreCajaAnual { get; set; }
}

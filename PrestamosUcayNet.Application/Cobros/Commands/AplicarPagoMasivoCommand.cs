using System;
using System.Collections.Generic;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Cobros.Commands;

/// <summary>
/// Comando para aplicar una recaudación masiva de amortizaciones
/// </summary>
public class AplicarPagoMasivoCommand
{
    public string? LoteId { get; set; }
    public string OrigenNomina { get; set; } = null!;
    public string MetodoPago { get; set; } = null!;
    public DateTime FechaCobro { get; set; }
    public List<DetallePagoMasivoDto> Detalles { get; set; } = new();
}

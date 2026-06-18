using System;

namespace PrestamosUcayNet.Application.Cobros.Commands;

/// <summary>
/// Comando para aplicar un pago individual a una amortización específica
/// </summary>
public class AplicarPagoIndividualCommand
{
    public int PrestamoId { get; set; }
    public int? NumeroPago { get; set; }
    public int? AmortizacionId { get; set; }
    public int? PrestamoDirectoId { get; set; }
    public bool EsPrestamoDirecto { get; set; }
    public decimal MontoRecibido { get; set; }
    public string MetodoPago { get; set; } = null!; // "Efectivo", "Transferencia", "Cheque", "Nomina"
    public string? Referencia { get; set; }
    public DateTime FechaPago { get; set; }
}

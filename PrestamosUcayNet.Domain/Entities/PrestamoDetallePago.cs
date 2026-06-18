using System;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Registra la distribución detallada de un cobro sobre una cuota de amortización específica
/// </summary>
public partial class PrestamoDetallePago
{
    public int DetallePagoId { get; set; }
    public int CobroId { get; set; }
    public int PrestamoId { get; set; }
    public int? AmortizacionId { get; set; }
    public int? PrestamoDirectoId { get; set; }
    public decimal MontoCapital { get; set; }
    public decimal MontoInteres { get; set; }
    public decimal PagoExtraCapital { get; set; }
    public decimal MontoTotal { get; set; }

    public virtual PrestamoCobro Cobro { get; set; } = null!;
    public virtual Prestamo Prestamo { get; set; } = null!;
    public virtual PrestamosAmortizacione? Amortizacion { get; set; }
    public virtual PrestamoDirecto? PrestamoDirecto { get; set; }
}

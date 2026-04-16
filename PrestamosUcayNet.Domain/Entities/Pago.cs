using System;

namespace PrestamosUcayNet.Domain.Entities;

public partial class Pago
{
    public int Id { get; set; }
    
    public int PrestamoId { get; set; }
    
    public DateTime FechaPago { get; set; }
    
    public decimal MontoPagado { get; set; }
    
    // Ordinario, Extraordinario, Liquidación
    public string TipoPago { get; set; } = null!;
    
    // Nómina, Transferencia, Efectivo
    public string MetodoPago { get; set; } = null!;
    
    public string? Referencia { get; set; }
    
    public string? Notas { get; set; }

    public virtual Prestamo Prestamo { get; set; } = null!;
}

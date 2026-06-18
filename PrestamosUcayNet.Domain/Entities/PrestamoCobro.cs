using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Representa la transacción general de cobro o pago de préstamos
/// </summary>
public partial class PrestamoCobro
{
    public int CobroId { get; set; }
    public int PrestamoId { get; set; }
    public DateTime FechaCobro { get; set; }
    public decimal MontoTotal { get; set; }
    public string MetodoPago { get; set; } = null!;
    public string? Referencia { get; set; }
    public string? UsuarioId { get; set; }
    public string? OrigenNomina { get; set; }

    public virtual Prestamo Prestamo { get; set; } = null!;
    public virtual ICollection<PrestamoDetallePago> PrestamoDetallePagos { get; set; } = new List<PrestamoDetallePago>();
}

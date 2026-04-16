using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class TiposPago
{
    /// <summary>
    /// Identificador del Tipo de Pago
    /// </summary>
    public int TipoPagoId { get; set; }

    /// <summary>
    /// Descripción Tipo de Pago
    /// </summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>
    /// Número de Días para la frecuencia de pago
    /// </summary>
    public int IntervaloDiario { get; set; }

    /// <summary>
    /// Número de Meses para la frecuencia del pago
    /// </summary>
    public int IntervaloMensual { get; set; }

    /// <summary>
    /// Intervalo Anual 
    /// </summary>
    public int IntervaloAnual { get; set; }

    /// <summary>
    /// Níumero de Pagos por año
    /// </summary>
    public int FrecuenciaAnual { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}

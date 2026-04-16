using System;
using System.Collections.Generic;
using PrestamosUcayNet.Domain.Enums;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Desfine los Tipo de Préstamos
/// </summary>
public partial class TipoPrestamo
{
    /// <summary>
    /// Identificador del Tipo de Préstamo
    /// </summary>
    public int TipoPrestamoId { get; set; }

    /// <summary>
    /// Descripción del Tipo Préstamo 
    /// </summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>
    /// Clasificación del Tipo de Préstamo
    /// </summary>
    public ClasificacionTipoPrestamo Clasificacion { get; set; }

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}

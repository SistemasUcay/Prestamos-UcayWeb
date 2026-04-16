using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Descripción de Puestos
/// </summary>
public partial class Puesto
{
    /// <summary>
    /// Registo ID del Puesto
    /// </summary>
    public int PuestoId { get; set; }

    /// <summary>
    /// Descripción del Puesto
    /// </summary>
    public string? Descripcion { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}

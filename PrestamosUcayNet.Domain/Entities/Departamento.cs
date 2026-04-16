using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class Departamento
{
    /// <summary>
    /// Identificador del Departamento
    /// </summary>
    public int DepartamentoId { get; set; }

    /// <summary>
    /// Descripción del Departamento
    /// </summary>
    public string? Descripcion { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}

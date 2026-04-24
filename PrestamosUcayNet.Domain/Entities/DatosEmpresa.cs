using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class DatosEmpresa
{
    /// <summary>
    /// Identificador de la Empresa o Caja de Ahorro
    /// </summary>
    public int NumEmpresa { get; set; }

    /// <summary>
    /// Razón Social de la Empresa
    /// </summary>
    public string RazonSocial { get; set; } = null!;

    /// <summary>
    /// Nombre Corto de la Empresa
    /// </summary>
    public string? NombreCorto { get; set; }

    /// <summary>
    /// Domicilio Completo de la Empresa
    /// </summary>
    public string? Direccion { get; set; }

    /// <summary>
    /// Código Postal del domicilio de la empresa
    /// </summary>
    public string? CodPostal { get; set; }

    /// <summary>
    /// RFC / NIT
    /// </summary>
    public string Rfc { get; set; } = null!;

    /// <summary>
    /// Teléfono de contacto de la empresa
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Correo electrónico de la empresa
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Representante legal de la empresa
    /// </summary>
    public string? RepresentanteLegal { get; set; }

    /// <summary>
    /// Indica si esta entidad opera como Caja de Ahorro
    /// </summary>
    public bool IsCajaDeAhorro { get; set; }

    /// <summary>
    /// Estado de la empresa (Activa/Inactiva)
    /// </summary>
    public bool Activa { get; set; }

    /// <summary>
    /// Fondo disponible actual (en caso de ser fondeadora)
    /// </summary>
    public decimal? SaldoActual { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    public virtual ICollection<CondicionCredito> CondicionesCredito { get; set; } = new List<CondicionCredito>();

    public virtual ICollection<Prestamo> PrestamosFondeados { get; set; } = new List<Prestamo>();
}

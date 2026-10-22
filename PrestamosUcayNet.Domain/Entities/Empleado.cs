using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class Empleado
{
    /// <summary>
    /// Identidicador el Empleado
    /// </summary>
    public int EmpleadoId { get; set; }

    /// <summary>
    /// Nombre(s) del Emplado
    /// </summary>
    public string Nombres { get; set; } = null!;

    /// <summary>
    /// Apellido Paterno
    /// </summary>
    public string ApellidoPaterno { get; set; } = null!;

    /// <summary>
    /// Apellido Materno
    /// </summary>
    public string? ApellidoMaterno { get; set; }

    /// <summary>
    /// RFC
    /// </summary>
    public string Rfc { get; set; } = null!;

    /// <summary>
    /// CURP
    /// </summary>
    public string? Curp { get; set; }

    /// <summary>
    /// Fecha de Nacimiento
    /// </summary>
    public DateTime Nacimiento { get; set; }

    /// <summary>
    /// Número del sistema de nómina del empleado
    /// </summary>
    public int NumeroNomina { get; set; }

    /// <summary>
    /// Nombre de la calle y número
    /// </summary>
    public string Domicilio { get; set; } = null!;

    /// <summary>
    /// Código Postal del Domicilio
    /// </summary>
    public int CodigoPostal { get; set; }

    /// <summary>
    /// Número del Celular
    /// </summary>
    public string? Celular { get; set; }

    /// <summary>
    /// Correo Electrónico
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Cuenta Bancaria de Depósitos
    /// </summary>
    public string? CuentaBancaria { get; set; }

    /// <summary>
    /// Número CLABE de la Cuenta de Depósito
    /// </summary>
    public string? CuentaClabe { get; set; }

    /// <summary>
    /// Fecha de Ingreso a la Empresa
    /// </summary>
    public DateTime? FechaIngreso { get; set; }

    /// <summary>
    /// Sueldo Nominal
    /// </summary>
    public decimal? SueldoNominal { get; set; }

    /// <summary>
    /// Sueldo Laboral
    /// </summary>
    public decimal? SueldoLaboral { get; set; }

    /// <summary>
    /// ID de la Empresa donde labora
    /// </summary>
    public int EmpresaId { get; set; }

    /// <summary>
    /// ID del Departamento 
    /// </summary>
    public int DepartamentoId { get; set; }

    /// <summary>
    /// Puesto donde labora el empleado
    /// </summary>
    public int PuestoId { get; set; }

    /// <summary>
    /// Tipo de Pago de la Nómina
    /// </summary>
    public int TipoPagoId { get; set; }

    /// <summary>
    /// Importe para la Caja de Ahorro
    /// </summary>
    public decimal ImporteCajaAhorro { get; set; }

    /// <summary>
    /// Inidica si el empleado es directo de UCAY
    /// </summary>
    public bool EmpleadoUcay { get; set; }

    public virtual DatosEmpresa Empresa { get; set; } = null!;

    public virtual Departamento Departamento { get; set; } = null!;

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual ICollection<PeriodoVacacional> PeriodosVacacionales { get; set; } = new List<PeriodoVacacional>();

    public virtual ICollection<SolicitudVacaciones> SolicitudesVacaciones { get; set; } = new List<SolicitudVacaciones>();

    public virtual Puesto Puesto { get; set; } = null!;

    public virtual TiposPago TipoPago { get; set; } = null!;
}


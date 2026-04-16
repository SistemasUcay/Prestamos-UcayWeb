using System;

namespace PrestamosUcayNet.Application.DTOs;

public class EmpleadoListDto
{
    public int EmpleadoId { get; set; }
    public string NombreCompleto => $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}".Trim();
    public string Nombres { get; set; } = null!;
    public string ApellidoPaterno { get; set; } = null!;
    public string? ApellidoMaterno { get; set; }
    public string Rfc { get; set; } = null!;
    public int NumeroNomina { get; set; }
    public string EmpresaNombre { get; set; } = null!;
    public string DepartamentoNombre { get; set; } = null!;
    public string PuestoNombre { get; set; } = null!;
    public bool EmpleadoUcay { get; set; }
}

public class EmpleadoDetailDto : EmpleadoListDto
{
    public string? Curp { get; set; }
    public DateTime Nacimiento { get; set; }
    public string Domicilio { get; set; } = null!;
    public int CodigoPostal { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }
    public string? CuentaBancaria { get; set; }
    public string? CuentaClabe { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public decimal? SueldoNominal { get; set; }
    public decimal? SueldoLaboral { get; set; }
    public int EmpresaId { get; set; }
    public int DepartamentoId { get; set; }
    public int PuestoId { get; set; }
    public int TipoPagoId { get; set; }
    public decimal ImporteCajaAhorro { get; set; }
}

public class EmpleadoCreateUpdateDto
{
    public string Nombres { get; set; } = null!;
    public string ApellidoPaterno { get; set; } = null!;
    public string? ApellidoMaterno { get; set; }
    public string Rfc { get; set; } = null!;
    public string? Curp { get; set; }
    public DateTime? Nacimiento { get; set; }
    public int NumeroNomina { get; set; }
    public string Domicilio { get; set; } = null!;
    public int CodigoPostal { get; set; }
    public string? Celular { get; set; }
    public string? Email { get; set; }
    public string? CuentaBancaria { get; set; }
    public string? CuentaClabe { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public decimal? SueldoNominal { get; set; }
    public decimal? SueldoLaboral { get; set; }
    public int EmpresaId { get; set; }
    public int DepartamentoId { get; set; }
    public int PuestoId { get; set; }
    public int TipoPagoId { get; set; }
    public decimal ImporteCajaAhorro { get; set; }
    public bool EmpleadoUcay { get; set; }
    public bool Activo { get; set; } = true;
}

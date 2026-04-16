using System;

namespace PrestamosUcayNet.Application.DTOs;

public class EmpresaListDto
{
    public int NumEmpresa { get; set; }
    public string RazonSocial { get; set; } = null!;
    public string? NombreCorto { get; set; }
    public string Rfc { get; set; } = null!;
    public bool IsCajaDeAhorro { get; set; }
    public bool Activa { get; set; }
    public decimal? SaldoActual { get; set; }
}

public class EmpresaDetailDto : EmpresaListDto
{
    public string? Direccion { get; set; }
    public string? CodPostal { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? RepresentanteLegal { get; set; }
}

public class EmpresaCreateUpdateDto
{
    public string RazonSocial { get; set; } = null!;
    public string? NombreCorto { get; set; }
    public string? Direccion { get; set; }
    public string? CodPostal { get; set; }
    public string Rfc { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? RepresentanteLegal { get; set; }
    public bool IsCajaDeAhorro { get; set; }
    public bool Activa { get; set; }
    public decimal? SaldoActual { get; set; }
}

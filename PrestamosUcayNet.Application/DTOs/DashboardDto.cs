using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Application.DTOs;

public class DashboardDto
{
    public decimal TotalColocado { get; set; }
    public decimal SaldoVigente { get; set; }
    public decimal SaldoVencido { get; set; }
    public int PrestamosActivosCount { get; set; }
    public int SolicitudesTramiteCount { get; set; }
    public decimal TasaMorosidad { get; set; }
    public decimal RecuperacionMensual { get; set; }

    public List<FondeadoraColocacionDto> DistribucionFondeadora { get; set; } = new();
    public List<MensualColocacionDto> EvolucionColocacion { get; set; } = new();
    public List<EstadoCarteraFondeadoraDto> EstadoCarteraFondeadora { get; set; } = new();
    public List<TopEmpleadoSaldoDto> TopEmpleados { get; set; } = new();
    public List<PrestamoListDto> SolicitudesRecientes { get; set; } = new();
    public List<PrestamoListDto> PrestamosCanceladosRechazados { get; set; } = new();
    public List<PrestamoListDto> PendientesEntrega { get; set; } = new();
}

public class FondeadoraColocacionDto
{
    public int FondeadoraId { get; set; }
    public string NombreFondeadora { get; set; } = null!;
    public string NombreCorto { get; set; } = null!;
    public decimal MontoColocado { get; set; }
    public double Porcentaje { get; set; }
}

public class MensualColocacionDto
{
    public string Mes { get; set; } = null!; // Ej: "Ene 2026"
    public int Anio { get; set; }
    public int MesNumero { get; set; }
    public int CantidadPrestamos { get; set; }
    public decimal MontoColocado { get; set; }
}

public class EstadoCarteraFondeadoraDto
{
    public int FondeadoraId { get; set; }
    public string NombreFondeadora { get; set; } = null!;
    public string NombreCorto { get; set; } = null!;
    public decimal TotalColocado { get; set; }
    public decimal SaldoVigente { get; set; }
    public decimal SaldoVencido { get; set; }
    public decimal TasaMorosidad { get; set; }
}

public class TopEmpleadoSaldoDto
{
    public int EmpleadoId { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string NombreEmpresa { get; set; } = null!;
    public decimal SaldoPendiente { get; set; }
}

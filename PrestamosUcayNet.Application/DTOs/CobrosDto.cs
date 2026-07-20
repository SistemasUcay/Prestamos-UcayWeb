using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Application.DTOs;

public class AmortizacionPendienteDto
{
    public int AmortizacionId { get; set; }
    public int PrestamoId { get; set; }
    public int NumeroPago { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal ImportePago { get; set; }
    public decimal ImportePagoCapital { get; set; }
    public decimal ImportePagoInteres { get; set; }
    public decimal PagoExtraCapital { get; set; }
    public decimal SaldoFinal { get; set; }
    public bool StatusPago { get; set; }

    public bool EsPrestamoDirecto { get; set; }
    public int? PrestamoDirectoId { get; set; }
    public string? DescripcionPrestamoDirecto { get; set; }
    public decimal? MontoSolicitadoDirecto { get; set; }
    public decimal? MontoInteresesDirecto { get; set; }
    public decimal? TotalPagarDirecto { get; set; }
    public DateTime? FechaPagoDirecto { get; set; }
    public bool? StatusPagoDirecto { get; set; }

    // Datos del Empleado / Empresa para búsquedas y filtros
    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = null!;
    public int EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = null!;
    public string? Folio { get; set; }
    public string TipoPrestamo { get; set; } = null!; // "Nómina" o "Directo"
}

public class BulkPaymentResultDto
{
    public string? LoteId { get; set; }
    public int TotalRegistros { get; set; }
    public int Exitosos { get; set; }
    public int Fallidos { get; set; }
    public List<string> DetallesErrores { get; set; } = new();
}

public class BulkCancelResultDto
{
    public string LoteId { get; set; } = string.Empty;
    public int TotalCobros { get; set; }
    public int Cancelados { get; set; }
    public int Fallidos { get; set; }
    public List<string> DetallesErrores { get; set; } = new();
}

public class DetallePagoMasivoDto
{
    public int PrestamoId { get; set; }
    public int? NumeroPago { get; set; }
    public int? AmortizacionId { get; set; }
    public int? PrestamoDirectoId { get; set; }
    public bool EsPrestamoDirecto { get; set; }
    public decimal MontoAplicado { get; set; }
}

public class CobroAplicadoDto
{
    public int CobroId { get; set; }
    public int PrestamoId { get; set; }
    public DateTime FechaCobro { get; set; }
    public decimal MontoTotal { get; set; }
    public string MetodoPago { get; set; } = null!;
    public string? Referencia { get; set; }
    public string? UsuarioId { get; set; }
    public string? OrigenNomina { get; set; }
    public string? LoteId { get; set; }
    public bool EsPrestamoDirecto { get; set; }
    public int? NumeroPago { get; set; }
    public int? AmortizacionId { get; set; }
    public int? PrestamoDirectoId { get; set; }
    public string Concepto { get; set; } = string.Empty;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrestamosUcayNet.Application.DTOs;

public class SolicitudDetalleDto
{
    public int Id { get; set; }
    public string NombreEmpleado { get; set; } = null!;
    public string NombreEmpresaEmpleadora { get; set; } = null!;
    public string NombreFondeadora { get; set; } = null!;
    public string NombreProducto { get; set; } = null!;
    public decimal MontoSolicitado { get; set; }
    public int PlazoMeses { get; set; }
    public string Motivo { get; set; } = null!;
    public string EstadoSolicitud { get; set; } = null!;
    public DateTime FechaSolicitud { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? MotivoCancelacion { get; set; }
}

public class AprobarSolicitudDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SolicitudId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal MontoAprobado { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }
}

public class RechazarSolicitudDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SolicitudId { get; set; }

    [Required]
    [MaxLength(500)]
    public string MotivoRechazo { get; set; } = null!;
}

public class CancelarSolicitudDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SolicitudId { get; set; }

    [Required]
    [MaxLength(500)]
    public string MotivoCancelacion { get; set; } = null!;
}

public class EditarSolicitudDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SolicitudId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CondicionCreditoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto solicitado debe ser mayor a cero.")]
    public decimal MontoSolicitado { get; set; }

    [Required]
    [Range(1, 360, ErrorMessage = "El plazo debe ser entre 1 y 360 meses.")]
    public int PlazoMeses { get; set; }

    [MaxLength(500)]
    public string Motivo { get; set; } = string.Empty;
}

public class ActivarPrestamoDto
{
    public int SolicitudId { get; set; }
    public DateTime FechaEntrega { get; set; }
    public string MetodoEntrega { get; set; } = null!; // Transferencia, Cheque, Efectivo
    public string Referencia { get; set; } = null!;
    public string? Folio { get; set; }
}

/// <summary>
/// DTO ligero para la lista/grilla de préstamos
/// </summary>
public class PrestamoListDto
{
    public int PrestamoId { get; set; }
    public string? Folio { get; set; }
    public string NombreEmpleado { get; set; } = null!;
    public string NombreFondeadora { get; set; } = null!;
    public string TipoPrestamoDescripcion { get; set; } = null!;
    public string TipoPagoDescripcion { get; set; } = null!;
    public decimal MontoAprobado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public int PlazoMeses { get; set; }
    public int NumeroCuotas { get; set; }
    public decimal ImporteCuotas { get; set; }
    public decimal TasaInteresMensual { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaAprobado { get; set; }
    public DateTime? FechaDeposito { get; set; }

    /// <summary>
    /// Etiqueta legible del estado del préstamo
    /// </summary>
    public string EstadoDescripcion => Estado switch
    {
        "A" => "Activo",
        "P" => "Pagado",
        "L" => "Liquidado",
        "C" => "Cancelado",
        "S" => "Suspendido",
        _ => Estado
    };
}

/// <summary>
/// DTO completo con detalle del préstamo y tabla de amortización
/// </summary>
public class PrestamoDetalleDto
{
    public int Id { get; set; }
    public string? Folio { get; set; }
    public string NombreEmpleado { get; set; } = null!;
    public string NombreFondeadora { get; set; } = null!;
    public string TipoPrestamoDescripcion { get; set; } = null!;
    public string TipoPagoDescripcion { get; set; } = null!;
    public decimal MontoAprobado { get; set; }
    public decimal TasaInteresAnual { get; set; }
    public decimal TasaInteresMensual { get; set; }
    public int PlazoMeses { get; set; }
    public int NumeroCuotas { get; set; }
    public decimal ImporteCuotas { get; set; }
    public string DestinoCredito { get; set; } = null!;
    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaAprobado { get; set; }
    public DateTime? FechaDeposito { get; set; }
    public DateTime InicioDescuento { get; set; }
    public DateTime FinalDescuento { get; set; }
    public string Estado { get; set; } = null!;
    public decimal TotalPagar { get; set; }
    public decimal SaldoPendiente { get; set; }
    public decimal SueldoLaboral { get; set; }
    public bool AvalCredito { get; set; }
    public string? MetodoEntrega { get; set; }
    public string? ReferenciaBancaria { get; set; }
    public List<AmortizacionDto> TablaAmortizacion { get; set; } = new();

    public string EstadoDescripcion => Estado switch
    {
        "A" => "Activo",
        "P" => "Pagado",
        "L" => "Liquidado",
        "C" => "Cancelado",
        "S" => "Suspendido",
        _ => Estado
    };
}

/// <summary>
/// DTO para actualizar campos editables de un préstamo
/// </summary>
public class PrestamoUpdateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int PrestamoId { get; set; }

    [MaxLength(250)]
    public string? DestinoCredito { get; set; }

    [MaxLength(50)]
    public string? MetodoEntrega { get; set; }

    [MaxLength(100)]
    public string? ReferenciaBancaria { get; set; }

    [MaxLength(50)]
    public string? Folio { get; set; }
}

public class AmortizacionDto
{
    public int NumeroPago { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal ImportePago { get; set; }
    public decimal ImporteCapital { get; set; }
    public decimal ImporteInteres { get; set; }
    public decimal SaldoFinal { get; set; }
    public bool Pagado { get; set; }
}

public class RegistrarPagoDto
{
    public int PrestamoId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string TipoPago { get; set; } = "Ordinario"; // Ordinario, Extraordinario, Liquidacion
    public string MetodoPago { get; set; } = "Nomina";
    public string? Referencia { get; set; }
    
    // Si es extraordinario
    public bool ReducirPlazo { get; set; } = true;
}

public class SolicitudPausaDto
{
    public int PrestamoId { get; set; }
    public int MesesPausa { get; set; }
    public string Motivo { get; set; } = null!;
}

/// <summary>
/// DTO para crear un préstamo directamente sin pasar por solicitud
/// </summary>
public class PrestamoCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int EmpleadoId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int FondeadoraId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int TipoPrestamoId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int TipoPagoId { get; set; }

    public int? CondicionCreditoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal MontoAprobado { get; set; }

    [Required]
    [Range(1, 360, ErrorMessage = "El plazo debe estar entre 1 y 360 meses.")]
    public int PlazoMeses { get; set; }

    [Required]
    [Range(0.001, 100, ErrorMessage = "La tasa mensual debe ser mayor a cero.")]
    public decimal TasaInteresMensual { get; set; }

    [Required]
    [MaxLength(250)]
    public string DestinoCredito { get; set; } = null!;

    [MaxLength(50)]
    public string? MetodoEntrega { get; set; }

    [MaxLength(100)]
    public string? ReferenciaBancaria { get; set; }

    [MaxLength(50)]
    public string? Folio { get; set; }

    public DateTime FechaEntrega { get; set; } = DateTime.Today;

    public int? EmpleadoIdAval { get; set; }
}

/// <summary>
/// DTO para simular la tabla de amortización antes de crear el préstamo
/// </summary>
public class SimularAmortizacionDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal MontoAprobado { get; set; }

    [Required]
    [Range(1, 360, ErrorMessage = "El plazo debe estar entre 1 y 360 meses.")]
    public int PlazoMeses { get; set; }

    [Required]
    [Range(0.001, 100, ErrorMessage = "La tasa anual debe ser mayor a cero.")]
    public decimal TasaInteresAnual { get; set; }

    public DateTime FechaEntrega { get; set; } = DateTime.Today;

    /// <summary>
    /// Número de días entre cada pago según la frecuencia del empleado (ej: 7=semanal, 14=catorcenal, 15=quincenal, 30=mensual)
    /// </summary>
    public int IntervaloDiario { get; set; } = 30;

    /// <summary>
    /// Número de pagos que realiza el empleado por año (ej: 52=semanal, 26=catorcenal, 24=quincenal, 12=mensual)
    /// </summary>
    public int FrecuenciaAnual { get; set; } = 12;
}

/// <summary>
/// Resultado de la simulación de amortización con tabla y resumen
/// </summary>
public class AmortizacionPreviewResultDto
{
    public List<AmortizacionDto> TablaAmortizacion { get; set; } = [];
    public decimal PagosAdicionalesCapital { get; set; }
    public decimal CapitalAmortizable { get; set; }
    public decimal TotalIntereses { get; set; }
    public decimal TotalPagar { get; set; }
    public decimal TasaMensual { get; set; }
    public int DiasPlayzo { get; set; }
    public DateTime FechaUltimoPago { get; set; }
    public decimal CuotaMensual { get; set; }

    /// <summary>
    /// Tasa de interés aplicada por período de pago (según frecuencia)
    /// </summary>
    public decimal TasaPorPeriodo { get; set; }

    /// <summary>
    /// Número total de pagos generados según la frecuencia
    /// </summary>
    public int NumeroPagos { get; set; }
}

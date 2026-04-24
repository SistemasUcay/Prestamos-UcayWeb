using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class Prestamo
{
    /// <summary>
    /// Identificador del Préstamo
    /// </summary>
    public int PrestamoId { get; set; }

    /// <summary>
    /// Fecha de Solicitud del Préstamo
    /// </summary>
    public DateTime FechaSolicitud { get; set; }

    /// <summary>
    /// Fuente de Fondeo Proveedor del capital para el financiamiento
    /// </summary>
    public int FuenteFondeoId { get; set; }

    /// <summary>
    /// ID del Empleador que solicita el Préstamo
    /// </summary>
    public int EmpleadoId { get; set; }

    /// <summary>
    /// Sueldo Laboral del Empleado al momento de Solicitar el Préstamo
    /// </summary>
    public decimal SueldoLaboral { get; set; }

    /// <summary>
    /// ID del Tipo de Préstamo
    /// </summary>
    public int TipoPrestamoId { get; set; }

    /// <summary>
    /// Tipos de Plazo: [1]: Corto Plazo [2]: Largo Plazo [3]: Hipotecario
    /// </summary>
    public int? TipoPlazo { get; set; }

    /// <summary>
    /// Tasa de Interes Anual del Préstamo
    /// </summary>
    public decimal TasaInteresAnual { get; set; }

    /// <summary>
    /// Importe del Préstamo Autorizado/Entregado
    /// </summary>
    public decimal MontoAprobado { get; set; }

    /// <summary>
    /// Plazo del Préstamo en Meses
    /// </summary>
    public int PlazoMeses { get; set; }

    /// <summary>
    /// Tasa de Interes Mensual del Préstamo
    /// </summary>
    public decimal TasaInteresMensual { get; set; }

    /// <summary>
    /// Definición del destino del Préstamo
    /// </summary>
    public string DestinoCredito { get; set; } = null!;

    /// <summary>
    /// Tipo de Pago o Amortización del Préstamo (Describe la frecuencia del Pago)
    /// </summary>
    public int TipoPagoId { get; set; }

    /// <summary>
    /// Número de Pagos o Cuotas Totales del Préstamo
    /// </summary>
    public int NumeroCuotas { get; set; }

    /// <summary>
    /// Importe o Monto del Pago en cada Frecuencia (Pago Programado)
    /// </summary>
    public decimal ImporteCuotas { get; set; }

    /// <summary>
    /// Fecha de Aporabación del Prestamo
    /// </summary>
    public DateTime FechaAprobado { get; set; }

    /// <summary>
    /// Fecha de Depósito del Préstamo
    /// </summary>
    public DateTime? FechaDeposito { get; set; }

    /// <summary>
    /// Fecha de inicio del descuento
    /// </summary>
    public DateTime InicioDescuento { get; set; }

    /// <summary>
    /// Fecha Final del Préstamo
    /// </summary>
    public DateTime FinalDescuento { get; set; }

    /// <summary>
    /// Indica si el Préstamo cuenta con Aval
    /// </summary>
    public bool AvalCredito { get; set; }

    /// <summary>
    /// ID del Empleado que será el Aval
    /// </summary>
    public int EmpleadoIdaval { get; set; }

    /// <summary>
    /// Status del Préstamo 
    /// </summary>
    public string Estado { get; set; } = null!;

    /// <summary>
    /// Método de entrega del dinero (transferencia, cheque, efectivo)
    /// </summary>
    public string? MetodoEntrega { get; set; }

    /// <summary>
    /// Referencia bancaria de la entrega
    /// </summary>
    public string? ReferenciaBancaria { get; set; }

    /// <summary>
    /// Folio del préstamo
    /// </summary>
    public string? Folio { get; set; }

    public decimal TotalPagar { get; set; }
    public decimal SaldoPendiente { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;

    public virtual DatosEmpresa FuenteFondeo { get; set; } = null!;

    public virtual ICollection<PrestamosAmortizacione> PrestamosAmortizaciones { get; set; } = new List<PrestamosAmortizacione>();

    public virtual TiposPago TipoPago { get; set; } = null!;

    public virtual TipoPrestamo TipoPrestamo { get; set; } = null!;
}

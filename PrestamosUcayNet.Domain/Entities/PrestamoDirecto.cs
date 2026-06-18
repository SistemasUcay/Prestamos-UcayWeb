using System;

namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Define los Préstamos Directos asociados a una Solicitud de Préstamo
/// Los préstamos directos se pagan en una sola exhibición al vencimiento
/// </summary>
public partial class PrestamoDirecto
{
    /// <summary>
    /// Identificador del Préstamo Directo
    /// </summary>
    public int NumPrestamoDirecto { get; set; }

    /// <summary>
    /// ID del Préstamo principal al que pertenece este préstamo directo
    /// </summary>
    public int PrestamoId { get; set; }

    /// <summary>
    /// Descripción o concepto del Préstamo Directo
    /// </summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>
    /// Monto solicitado del Préstamo Directo
    /// </summary>
    public decimal MontoSolicitado { get; set; }

    /// <summary>
    /// Tasa de interés mensual aplicable
    /// </summary>
    public decimal TasaMensual { get; set; }

    /// <summary>
    /// Fecha de pago único (vencimiento)
    /// </summary>
    public DateTime FechaPago { get; set; }

    /// <summary>
    /// Días de plazo desde la fecha de solicitud hasta el vencimiento
    /// </summary>
    public int DiazPlazo { get; set; }

    /// <summary>
    /// Monto de intereses calculados
    /// </summary>
    public decimal MontoIntereses { get; set; }

    /// <summary>
    /// Total a pagar (Capital + Intereses)
    /// </summary>
    public decimal TotalCapitalInteres { get; set; }

    /// <summary>
    /// Indica si el préstamo directo ya fue pagado
    /// </summary>
    public bool StatusPago { get; set; }

    /// <summary>
    /// Fecha en que se registró el cobro del préstamo directo
    /// </summary>
    public DateTime? FechaPagoReal { get; set; }

    /// <summary>
    /// Navegación al préstamo principal
    /// </summary>
    public virtual Prestamo Prestamo { get; set; } = null!;
}

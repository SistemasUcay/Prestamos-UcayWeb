using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class PrestamosAmortizacione
{
    /// <summary>
    /// Identificador de la Amortización
    /// </summary>
    public int AmortizacionId { get; set; }

    /// <summary>
    /// ID del Préstamo 
    /// </summary>
    public int PrestamoId { get; set; }

    /// <summary>
    /// Número del Pago o Cuota Programado
    /// </summary>
    public int NumeroPago { get; set; }

    /// <summary>
    /// Fecha de Pago Programado
    /// </summary>
    public DateTime FechaPago { get; set; }

    /// <summary>
    /// Feha de Inicio del Periodo para Nómina
    /// </summary>
    public DateTime InicioPeriodoPago { get; set; }

    /// <summary>
    /// Descripción del Pago
    /// </summary>
    public string DescripcionPago { get; set; } = null!;

    /// <summary>
    /// Tasa de interés del cálculo
    /// </summary>
    public decimal? TasaInteres { get; set; }

    /// <summary>
    /// Días del Cálculo para el Interés base del Cálculo SaldoInicial
    /// </summary>
    public int? DiasCalculo { get; set; }

    /// <summary>
    /// Importe del Saldo Inicial del Préstamo
    /// </summary>
    public decimal SaldoInicial { get; set; }

    /// <summary>
    /// Importe del Pago o Cuota Programada
    /// </summary>
    public decimal ImportePago { get; set; }

    /// <summary>
    /// Importe del Pago a Capital Programado 
    /// </summary>
    public decimal ImportePagoCapital { get; set; }

    /// <summary>
    /// Importe del Pago a Interés Programado 
    /// </summary>
    public decimal ImportePagoInteres { get; set; }

    public decimal PagoExtraCapital { get; set; }

    /// <summary>
    /// Saldo de Capital
    /// </summary>
    public decimal SaldoFinal { get; set; }

    /// <summary>
    /// Indica si el Pago ha sido aplicado
    /// </summary>
    public bool StatusPago { get; set; }

    public virtual Prestamo Prestamo { get; set; } = null!;
}

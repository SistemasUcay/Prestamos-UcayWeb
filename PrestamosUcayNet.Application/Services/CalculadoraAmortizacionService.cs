using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Domain.Interfaces;

namespace PrestamosUcayNet.Application.Services;

public class CalculadoraAmortizacionService : ICalculadoraAmortizacionService
{
    private readonly IRepository<Prestamo> _prestamoRepository;
    private readonly IRepository<PrestamosAmortizacione> _amortizacionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CalculadoraAmortizacionService(
        IRepository<Prestamo> prestamoRepository,
        IRepository<PrestamosAmortizacione> amortizacionRepository,
        IUnitOfWork unitOfWork)
    {
        _prestamoRepository = prestamoRepository;
        _amortizacionRepository = amortizacionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerarYGuardarTablaAmortizacionAsync(int prestamoId)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);
        if (prestamo == null) throw new ArgumentException("Préstamo no encontrado");

        decimal P = prestamo.MontoAprobado;
        int n = prestamo.PlazoMeses;
        decimal i = prestamo.TasaInteresMensual / 100m;

        decimal cuota;
        if (i == 0)
        {
            cuota = P / n;
        }
        else
        {
            decimal factor = (decimal)Math.Pow((double)(1 + i), n);
            cuota = P * (i * factor) / (factor - 1);
        }

        prestamo.ImporteCuotas = Math.Round(cuota, 2);
        
        var cuotas = new List<PrestamosAmortizacione>();
        decimal saldoAnterior = P;

        for (int k = 1; k <= n; k++)
        {
            decimal interesCuota = Math.Round(saldoAnterior * i, 2);
            decimal capitalCuota = Math.Round(cuota - interesCuota, 2);

            if (k == n)
            {
                capitalCuota = saldoAnterior;
                cuota = capitalCuota + interesCuota;
            }

            decimal saldoFinal = Math.Round(saldoAnterior - capitalCuota, 2);

            var fechaBase = prestamo.FechaAprobado != default ? prestamo.FechaAprobado : DateTime.Now;

            var amortizacion = new PrestamosAmortizacione
            {
                PrestamoId = prestamoId,
                NumeroPago = k,
                FechaPago = fechaBase.AddMonths(k),
                InicioPeriodoPago = fechaBase.AddMonths(k - 1),
                DescripcionPago = $"Cuota {k}/{n}",
                TasaInteres = prestamo.TasaInteresMensual,
                SaldoInicial = saldoAnterior,
                ImportePago = Math.Round(cuota, 2),
                ImportePagoCapital = capitalCuota,
                ImportePagoInteres = interesCuota,
                SaldoFinal = saldoFinal,
                StatusPago = false
            };
            
            cuotas.Add(amortizacion);
            saldoAnterior = saldoFinal;
        }

        prestamo.SaldoPendiente = P;
        prestamo.TotalPagar = cuotas.Sum(c => c.ImportePago);

        await _amortizacionRepository.AddRangeAsync(cuotas);
        _prestamoRepository.Update(prestamo);
        
        await _unitOfWork.CompleteAsync();
    }

    public async Task RecalcularYGuardarPorPagoAnticipadoAsync(int prestamoId, decimal pagoExtra, bool mantenerCuotaReducirPlazo)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);
        if (prestamo == null) throw new ArgumentException("Préstamo no encontrado");

        var cuotasExistentes = (await _amortizacionRepository.FindAsync(c => c.PrestamoId == prestamoId && !c.StatusPago)).ToList();
        
        decimal nuevoSaldo = Math.Max(0, prestamo.SaldoPendiente - pagoExtra);
        
        if (nuevoSaldo == 0)
        {
            _amortizacionRepository.RemoveRange(cuotasExistentes);
            prestamo.SaldoPendiente = 0;
            prestamo.Estado = "L"; // Liquidado
            _prestamoRepository.Update(prestamo);
            await _unitOfWork.CompleteAsync();
            return;
        }

        _amortizacionRepository.RemoveRange(cuotasExistentes);

        decimal i = prestamo.TasaInteresMensual / 100m;
        int cuotasRestantes = cuotasExistentes.Count;
        decimal n = cuotasRestantes;
        decimal C = prestamo.ImporteCuotas;
        decimal P = nuevoSaldo;

        if (mantenerCuotaReducirPlazo)
        {
            if (i > 0)
            {
                var numerador = 1 - (double)((P * i) / C);
                if (numerador > 0)
                {
                    n = (decimal)(-Math.Log(numerador) / Math.Log((double)(1 + i)));
                    cuotasRestantes = (int)Math.Ceiling(n);
                }
            }
            else
            {
                cuotasRestantes = (int)Math.Ceiling(P / C);
            }
        }
        else
        {
            if (i == 0)
            {
                C = P / cuotasRestantes;
            }
            else
            {
                decimal factor = (decimal)Math.Pow((double)(1 + i), cuotasRestantes);
                C = P * (i * factor) / (factor - 1);
            }
            prestamo.ImporteCuotas = Math.Round(C, 2);
        }

        var cuotas = new List<PrestamosAmortizacione>();
        decimal saldoAnterior = P;
        var fechaBase = DateTime.Now;

        for (int k = 1; k <= cuotasRestantes; k++)
        {
            decimal interesCuota = Math.Round(saldoAnterior * i, 2);
            decimal capitalCuota = Math.Round(C - interesCuota, 2);

            if (k == cuotasRestantes)
            {
                capitalCuota = saldoAnterior;
                C = capitalCuota + interesCuota;
            }

            decimal saldoFinal = Math.Round(saldoAnterior - capitalCuota, 2);

            var amortizacion = new PrestamosAmortizacione
            {
                PrestamoId = prestamoId,
                NumeroPago = k,
                FechaPago = fechaBase.AddMonths(k),
                InicioPeriodoPago = fechaBase.AddMonths(k - 1),
                DescripcionPago = $"Cuota {k}/{cuotasRestantes} (Recalculada)",
                TasaInteres = prestamo.TasaInteresMensual,
                SaldoInicial = saldoAnterior,
                ImportePago = Math.Round(C, 2),
                ImportePagoCapital = capitalCuota,
                ImportePagoInteres = interesCuota,
                SaldoFinal = saldoFinal,
                StatusPago = false
            };
            
            cuotas.Add(amortizacion);
            saldoAnterior = saldoFinal;
        }

        prestamo.SaldoPendiente = nuevoSaldo;
        await _amortizacionRepository.AddRangeAsync(cuotas);
        _prestamoRepository.Update(prestamo);
        
        await _unitOfWork.CompleteAsync();
    }

    public async Task PausarCuotasAsync(int prestamoId, int mesesPausa)
    {
        var cuotasPendientes = await _amortizacionRepository.FindAsync(c => c.PrestamoId == prestamoId && !c.StatusPago);
        
        foreach (var cuota in cuotasPendientes.OrderBy(c => c.NumeroPago))
        {
            cuota.FechaPago = cuota.FechaPago.AddMonths(mesesPausa);
            cuota.InicioPeriodoPago = cuota.InicioPeriodoPago.AddMonths(mesesPausa);
            cuota.DescripcionPago += $" (Pausada {mesesPausa} m)";
            _amortizacionRepository.Update(cuota);
        }

        await _unitOfWork.CompleteAsync();
    }

    /// <inheritdoc />
    public AmortizacionPreviewResultDto SimularAmortizacion(SimularAmortizacionDto dto)
    {
        decimal P = dto.MontoAprobado;
        int frecuenciaAnual = dto.FrecuenciaAnual > 0 ? dto.FrecuenciaAnual : 12;
        int intervaloDiario = dto.IntervaloDiario > 0 ? dto.IntervaloDiario : 30;

        // Número total de pagos según la frecuencia: plazo en meses × pagos por año ÷ 12
        int n = (int)Math.Ceiling(dto.PlazoMeses * frecuenciaAnual / 12.0);
        if (n < 1) n = 1;

        // Tasa de interés por período de pago
        decimal tasaPorPeriodo = dto.TasaInteresAnual / frecuenciaAnual / 100m;

        decimal cuota;
        if (tasaPorPeriodo == 0)
        {
            cuota = P / n;
        }
        else
        {
            decimal factor = (decimal)Math.Pow((double)(1 + tasaPorPeriodo), n);
            cuota = P * (tasaPorPeriodo * factor) / (factor - 1);
        }

        cuota = Math.Round(cuota, 2);

        var tabla = new List<AmortizacionDto>();
        decimal saldoAnterior = P;
        decimal totalIntereses = 0;
        decimal totalCapital = 0;

        for (int k = 1; k <= n; k++)
        {
            decimal interesCuota = Math.Round(saldoAnterior * tasaPorPeriodo, 2);
            decimal capitalCuota = Math.Round(cuota - interesCuota, 2);

            if (k == n)
            {
                capitalCuota = saldoAnterior;
                cuota = capitalCuota + interesCuota;
            }

            decimal saldoFinal = Math.Round(saldoAnterior - capitalCuota, 2);

            tabla.Add(new AmortizacionDto
            {
                NumeroPago = k,
                FechaPago = dto.FechaEntrega.AddDays(k * intervaloDiario),
                ImportePago = Math.Round(cuota, 2),
                ImporteCapital = capitalCuota,
                ImporteInteres = interesCuota,
                SaldoFinal = saldoFinal,
                Pagado = false
            });

            totalIntereses += interesCuota;
            totalCapital += capitalCuota;
            saldoAnterior = saldoFinal;
        }

        DateTime fechaUltimoPago = tabla.Count > 0 ? tabla[^1].FechaPago : dto.FechaEntrega;
        int diasPlazo = (int)(fechaUltimoPago - dto.FechaEntrega).TotalDays;

        return new AmortizacionPreviewResultDto
        {
            TablaAmortizacion = tabla,
            PagosAdicionalesCapital = 0,
            CapitalAmortizable = P,
            TotalIntereses = Math.Round(totalIntereses, 2),
            TotalPagar = Math.Round(totalCapital + totalIntereses, 2),
            TasaMensual = Math.Round(dto.TasaInteresAnual / 12m, 2),
            TasaPorPeriodo = Math.Round(tasaPorPeriodo * 100m, 4),
            NumeroPagos = n,
            DiasPlayzo = diasPlazo,
            FechaUltimoPago = fechaUltimoPago,
            CuotaMensual = tabla.Count > 0 ? tabla[0].ImportePago : 0
        };
    }
}

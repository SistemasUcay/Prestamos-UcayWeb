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

        // Número máximo de pagos según plazo (puede reducirse si hay cierre de caja)
        int nMax = (int)Math.Ceiling(dto.PlazoMeses * frecuenciaAnual / 12.0);
        if (nMax < 1) nMax = 1;

        // Tasa de interés por período de pago y tasa diaria para primer período irregular
        decimal tasaPorPeriodo = dto.TasaInteresAnual / frecuenciaAnual / 100m;
        decimal tasaDiaria     = dto.TasaInteresAnual / 12m / 30m / 100m;

        // ── Generador de fechas de pago ──────────────────────────────────────────────
        IEnumerable<DateTime> GenerarFechasPago()
        {
            if (dto.FechaInicioPeriodo is { } inicio)
            {
                DateTime cursor = inicio;
                while (cursor <= dto.FechaEntrega)
                    cursor = cursor.AddDays(intervaloDiario);
                while (true)
                {
                    yield return cursor;
                    cursor = cursor.AddDays(intervaloDiario);
                }
            }
            else
            {
                for (int k = 1; ; k++)
                    yield return dto.FechaEntrega.AddDays(k * intervaloDiario);
            }
        }

        // ── PASADA 1: determinar fechas reales y calcular ΣIntereses ────────────────
        // Se usa capital fijo (P / nMax) para decrementar el saldo y así obtener los
        // intereses reales sobre saldo decreciente. Si hay cierre de caja, el número
        // real de pagos puede ser menor a nMax y el saldo residual se absorbe en la
        // última cuota.
        var fechasPasada1 = new List<DateTime>();
        {
            decimal capitalFijo = tasaPorPeriodo == 0 ? P : Math.Round(P / nMax, 2);
            decimal saldo = P;
            using var en = GenerarFechasPago().GetEnumerator();

            for (int k = 1; k <= nMax && saldo > 0; k++)
            {
                en.MoveNext();
                DateTime fecha = en.Current;

                // Si la fecha supera el cierre, la última fecha ya registrada absorbe el resto
                if (dto.FechaCierreCaja.HasValue && fecha > dto.FechaCierreCaja.Value)
                    break;

                fechasPasada1.Add(fecha);
                saldo = Math.Round(saldo - capitalFijo, 2);
                if (saldo < 0) saldo = 0;
            }

            // Caso extremo: sin ninguna fecha válida (primera ya supera el cierre)
            if (fechasPasada1.Count == 0 && dto.FechaCierreCaja.HasValue)
                fechasPasada1.Add(dto.FechaCierreCaja.Value);
        }

        int nReal = fechasPasada1.Count;

        // Calcular ΣIntereses con saldo decreciente usando capital fijo real
        decimal totalInteresesPasada = 0;
        if (tasaPorPeriodo != 0 && nReal > 0)
        {
            decimal capitalFijoReal = Math.Round(P / nReal, 2);
            decimal saldo = P;
            for (int k = 0; k < nReal; k++)
            {
                decimal interes = k == 0
                    ? Math.Round(saldo * tasaDiaria * Math.Max(1, (fechasPasada1[0] - dto.FechaEntrega).Days), 2)
                    : Math.Round(saldo * tasaPorPeriodo, 2);
                totalInteresesPasada += interes;
                saldo = Math.Round(saldo - capitalFijoReal, 2);
                if (saldo < 0) saldo = 0;
            }
        }

        // ── Cuota promedio = (Capital + ΣIntereses) / nReal ─────────────────────────
        decimal cuota = nReal > 0
            ? Math.Round((P + totalInteresesPasada) / nReal, 2)
            : P;

        // Calcular préstamos directos
        var prestamosDirectosCalculados = new List<PrestamoDirectoDto>();
        decimal totalPrestamosDirectos = 0;
        if (dto.PrestamosDirectos is { Count: > 0 })
        {
            foreach (var pd in dto.PrestamosDirectos)
            {
                if (pd.MontoSolicitado > 0)
                {
                    int diasPlazoPD = pd.DiazPlazo > 0 ? pd.DiazPlazo : (int)(pd.FechaPago - dto.FechaEntrega).TotalDays;
                    // TasaMensual viene como porcentaje (ej. 2.5 = 2.5%); se divide entre 100 para obtener el factor.
                    // Si no viene tasa, se usa tasaPorPeriodo que ya está en factor decimal.
                    decimal tasaFactorPD = pd.TasaMensual > 0 ? pd.TasaMensual / 100m : tasaPorPeriodo;
                    decimal mesesPlazo = diasPlazoPD / 30m;
                    decimal intereses = Math.Round(pd.MontoSolicitado * tasaFactorPD * mesesPlazo, 2);
                    decimal total = pd.MontoSolicitado + intereses;

                    prestamosDirectosCalculados.Add(new PrestamoDirectoDto
                    {
                        Descripcion      = pd.Descripcion,
                        MontoSolicitado  = pd.MontoSolicitado,
                        TasaMensual      = pd.TasaMensual > 0 ? pd.TasaMensual : tasaPorPeriodo * 100m,
                        FechaPago        = pd.FechaPago,
                        DiazPlazo        = diasPlazoPD,
                        MontoIntereses   = intereses,
                        TotalCapitalInteres = total
                    });
                    totalPrestamosDirectos += total;
                }
            }
        }

        // ── PASADA 2: construir la tabla definitiva con la cuota promedio ────────────
        var tabla = new List<AmortizacionDto>();
        decimal saldoAnterior = P;
        decimal totalIntereses = 0;
        decimal totalCapital   = 0;

        for (int k = 0; k < nReal && saldoAnterior > 0; k++)
        {
            DateTime fechaPago = fechasPasada1[k];
            bool esUltima = k == nReal - 1;

            // Interés: días reales en el primer pago, tasa por período en el resto
            decimal interes = k == 0
                ? Math.Round(saldoAnterior * tasaDiaria * Math.Max(1, (fechaPago - dto.FechaEntrega).Days), 2)
                : Math.Round(saldoAnterior * tasaPorPeriodo, 2);

            decimal capital = Math.Round(cuota - interes, 2);

            // Última cuota: liquidar el saldo restante en su totalidad
            if (esUltima || capital >= saldoAnterior)
            {
                capital = saldoAnterior;
                cuota   = capital + interes;
            }

            decimal saldoFinal = Math.Round(saldoAnterior - capital, 2);

            tabla.Add(new AmortizacionDto
            {
                NumeroPago     = k + 1,
                FechaPago      = fechaPago,
                ImportePago    = Math.Round(cuota, 2),
                ImporteCapital = capital,
                ImporteInteres = interes,
                PagoExtra      = 0,
                SaldoFinal     = saldoFinal,
                Pagado         = false
            });

            totalIntereses += interes;
            totalCapital   += capital;
            saldoAnterior   = saldoFinal;
        }

        DateTime fechaUltimoPago = tabla.Count > 0 ? tabla[^1].FechaPago : dto.FechaEntrega;
        int diasPlazo = (int)(fechaUltimoPago - dto.FechaEntrega).TotalDays;

        decimal totalAmortizacion = Math.Round(totalCapital + totalIntereses, 2);
        decimal granTotal = totalAmortizacion + totalPrestamosDirectos;

        // La cuota que se muestra es el importe de la primera fila (uniforme en todas
        // las cuotas regulares; la última puede diferir por ajuste de saldo).
        decimal cuotaDisplay = tabla.Count > 0 ? tabla[0].ImportePago : 0;

        return new AmortizacionPreviewResultDto
        {
            TablaAmortizacion       = tabla,
            PrestamosDirectos       = prestamosDirectosCalculados,
            PagosAdicionalesCapital = 0,
            CapitalAmortizable      = P,
            TotalIntereses          = Math.Round(totalIntereses, 2),
            TotalPagar              = totalAmortizacion,
            TotalPrestamosDirectos  = totalPrestamosDirectos,
            GranTotal               = granTotal,
            TasaMensual             = Math.Round(dto.TasaInteresAnual / 12m, 2),
            TasaPorPeriodo          = Math.Round(tasaPorPeriodo * 100m, 4),
            NumeroPagos             = nReal,
            DiasPlayzo              = diasPlazo,
            FechaUltimoPago         = fechaUltimoPago,
            CuotaMensual            = cuotaDisplay
        };
    }
}

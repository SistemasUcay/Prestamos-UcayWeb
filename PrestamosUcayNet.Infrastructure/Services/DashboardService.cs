using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;

    public DashboardService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<DashboardDto> GetDashboardDataAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var today = DateTime.Today;
        var rangoFin = fechaFin?.Date ?? today;
        var rangoInicio = fechaInicio?.Date ?? new DateTime(today.AddMonths(-11).Year, today.AddMonths(-11).Month, 1);

        if (rangoInicio > rangoFin)
        {
            var tmp = rangoInicio;
            rangoInicio = rangoFin;
            rangoFin = tmp;
        }

        // 1. Obtener préstamos activos en memoria para cálculos seguros
        var prestamosActivos = await _context.Prestamos
            .Include(p => p.FuenteFondeo)
            .Include(p => p.PrestamosAmortizaciones)
            .Where(p => p.Estado == "A")
            .ToListAsync();

        var totalColocado = prestamosActivos.Sum(p => p.MontoAprobado);
        var prestamosActivosCount = prestamosActivos.Count;

        // Solicitudes en evaluación
        var solicitudesTramiteCount = await _context.Prestamos
            .CountAsync(p => p.Estado == "E");

        // Saldo vigente y vencido de préstamos activos
        var todasAmortizacionesActivas = prestamosActivos
            .SelectMany(p => p.PrestamosAmortizaciones)
            .Where(a => !a.StatusPago)
            .ToList();

        var saldoVigente = todasAmortizacionesActivas
            .Where(a => a.FechaPago >= today)
            .Sum(a => a.ImportePagoCapital);

        var saldoVencido = todasAmortizacionesActivas
            .Where(a => a.FechaPago < today)
            .Sum(a => a.ImportePagoCapital);

        decimal tasaMorosidad = 0;
        var totalPendiente = saldoVigente + saldoVencido;
        if (totalPendiente > 0)
        {
            tasaMorosidad = Math.Round((saldoVencido / totalPendiente) * 100m, 2);
        }

        // Recuperación Mensual: Cuotas aplicadas (StatusPago = true) programadas o pagadas en el mes actual
        var primerDiaMes = new DateTime(today.Year, today.Month, 1);
        var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
        
        var recuperacionMensual = await _context.PrestamosAmortizaciones
            .Where(a => a.StatusPago && a.FechaPago >= primerDiaMes && a.FechaPago <= ultimoDiaMes)
            .SumAsync(a => a.ImportePago);

        // 2. Distribución por Fondeadora (basado en préstamos activos)
        var distribucionFondeadora = prestamosActivos
            .GroupBy(p => new { p.FuenteFondeoId, NombreFondeadora = p.FuenteFondeo?.RazonSocial, NombreCorto = p.FuenteFondeo?.NombreCorto })
            .Select(g => new FondeadoraColocacionDto
            {
                FondeadoraId = g.Key.FuenteFondeoId,
                NombreFondeadora = g.Key.NombreFondeadora ?? "Sin Fondeadora",
                NombreCorto = string.IsNullOrWhiteSpace(g.Key.NombreCorto) ? (g.Key.NombreFondeadora ?? "Sin Fondeadora") : g.Key.NombreCorto!,
                MontoColocado = g.Sum(p => p.MontoAprobado)
            })
            .OrderByDescending(x => x.MontoColocado)
            .ToList();

        var totalColocadoActive = distribucionFondeadora.Sum(d => d.MontoColocado);
        foreach (var df in distribucionFondeadora)
        {
            df.Porcentaje = totalColocadoActive > 0 
                ? (double)Math.Round((df.MontoColocado / totalColocadoActive) * 100, 2) 
                : 0;
        }

        // 3. Evolución mensual por rango (solo préstamos activos por fecha de entrega)
        var firstDayStartMonth = new DateTime(rangoInicio.Year, rangoInicio.Month, 1);
        var firstDayEndMonth = new DateTime(rangoFin.Year, rangoFin.Month, 1);

        var prestamosMensuales = await _context.Prestamos
            .Where(p => p.Estado == "A"
                        && p.FechaDeposito.HasValue
                        && p.FechaDeposito.Value >= firstDayStartMonth
                        && p.FechaDeposito.Value <= rangoFin)
            .GroupBy(p => new { p.FechaDeposito!.Value.Year, p.FechaDeposito!.Value.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Cantidad = g.Count(),
                Monto = g.Sum(p => p.MontoAprobado)
            })
            .ToListAsync();

        var evolucionColocacion = new List<MensualColocacionDto>();
        string[] nombresMeses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

        var totalMeses = ((firstDayEndMonth.Year - firstDayStartMonth.Year) * 12) + firstDayEndMonth.Month - firstDayStartMonth.Month + 1;

        for (int i = 0; i < totalMeses; i++)
        {
            var targetMonth = firstDayStartMonth.AddMonths(i);
            var match = prestamosMensuales.FirstOrDefault(pm => pm.Year == targetMonth.Year && pm.Month == targetMonth.Month);
            evolucionColocacion.Add(new MensualColocacionDto
            {
                Mes = $"{nombresMeses[targetMonth.Month - 1]} {targetMonth.Year}",
                Anio = targetMonth.Year,
                MesNumero = targetMonth.Month,
                CantidadPrestamos = match?.Cantidad ?? 0,
                MontoColocado = match?.Monto ?? 0
            });
        }

        // 4. Estado de Cartera por Fondeadora
        var estadoCarteraFondeadora = prestamosActivos
            .Where(p => p.FuenteFondeo != null)
            .GroupBy(p => new { p.FuenteFondeoId, p.FuenteFondeo!.RazonSocial, p.FuenteFondeo.NombreCorto })
            .Select(g => {
                var total = g.Sum(p => p.MontoAprobado);
                var amortizacionesGrupo = g.SelectMany(p => p.PrestamosAmortizaciones).Where(a => !a.StatusPago).ToList();
                var vig = amortizacionesGrupo.Where(a => a.FechaPago >= today).Sum(a => a.ImportePagoCapital);
                var ven = amortizacionesGrupo.Where(a => a.FechaPago < today).Sum(a => a.ImportePagoCapital);
                var den = vig + ven;
                var moro = den > 0 ? Math.Round((ven / den) * 100m, 2) : 0m;
                return new EstadoCarteraFondeadoraDto
                {
                    FondeadoraId = g.Key.FuenteFondeoId,
                    NombreFondeadora = g.Key.RazonSocial ?? "",
                    NombreCorto = g.Key.NombreCorto ?? "",
                    TotalColocado = total,
                    SaldoVigente = vig,
                    SaldoVencido = ven,
                    TasaMorosidad = moro
                };
            })
            .ToList();

        // 5. Top 10 Empleados con Mayor Saldo Pendiente
        var topEmpleados = await _context.Prestamos
            .Include(p => p.Empleado)
            .ThenInclude(e => e.Empresa)
            .Where(p => p.Estado == "A" && p.SaldoPendiente > 0)
            .GroupBy(p => new { 
                p.EmpleadoId, 
                p.Empleado.Nombres, 
                p.Empleado.ApellidoPaterno, 
                p.Empleado.ApellidoMaterno,
                NombreEmpresa = p.Empleado.Empresa.NombreCorto ?? p.Empleado.Empresa.RazonSocial
            })
            .Select(g => new TopEmpleadoSaldoDto
            {
                EmpleadoId = g.Key.EmpleadoId,
                NombreCompleto = $"{g.Key.Nombres} {g.Key.ApellidoPaterno} {g.Key.ApellidoMaterno}".Trim(),
                NombreEmpresa = g.Key.NombreEmpresa,
                SaldoPendiente = g.Sum(p => p.SaldoPendiente)
            })
            .OrderByDescending(t => t.SaldoPendiente)
            .Take(10)
            .ToListAsync();

        // 6. Solicitudes Recientes (Últimas 10 en Evaluación)
        var solicitudesRecientesRaw = await _context.Prestamos
            .Include(p => p.Empleado)
            .Include(p => p.FuenteFondeo)
            .Include(p => p.TipoPrestamo)
            .Include(p => p.TipoPago)
            .Where(p => p.Estado == "E")
            .OrderByDescending(p => p.FechaSolicitud)
            .Take(10)
            .ToListAsync();

        var solicitudesRecientes = _mapper.Map<List<PrestamoListDto>>(solicitudesRecientesRaw);

        // 7. Préstamos Cancelados/Rechazados recientes
        var canceladosRechazadosRaw = await _context.Prestamos
            .Include(p => p.Empleado)
            .Include(p => p.FuenteFondeo)
            .Include(p => p.TipoPrestamo)
            .Include(p => p.TipoPago)
            .Where(p => (p.Estado == "C" || p.Estado == "R")
                        && p.FechaSolicitud >= rangoInicio
                        && p.FechaSolicitud <= rangoFin)
            .OrderByDescending(p => p.FechaSolicitud)
            .Take(30)
            .ToListAsync();

        var prestamosCanceladosRechazados = _mapper.Map<List<PrestamoListDto>>(canceladosRechazadosRaw);

        // 8. Préstamos Pendientes de Entrega (estado T)
        var pendientesEntregaRaw = await _context.Prestamos
            .Include(p => p.Empleado)
            .Include(p => p.FuenteFondeo)
            .Include(p => p.TipoPrestamo)
            .Include(p => p.TipoPago)
            .Where(p => p.Estado == "T")
            .OrderByDescending(p => p.FechaSolicitud)
            .Take(30)
            .ToListAsync();

        var pendientesEntrega = _mapper.Map<List<PrestamoListDto>>(pendientesEntregaRaw);

        return new DashboardDto
        {
            TotalColocado = totalColocado,
            SaldoVigente = saldoVigente,
            SaldoVencido = saldoVencido,
            PrestamosActivosCount = prestamosActivosCount,
            SolicitudesTramiteCount = solicitudesTramiteCount,
            TasaMorosidad = tasaMorosidad,
            RecuperacionMensual = recuperacionMensual,
            DistribucionFondeadora = distribucionFondeadora,
            EvolucionColocacion = evolucionColocacion,
            EstadoCarteraFondeadora = estadoCarteraFondeadora,
            TopEmpleados = topEmpleados,
            SolicitudesRecientes = solicitudesRecientes,
            PrestamosCanceladosRechazados = prestamosCanceladosRechazados,
            PendientesEntrega = pendientesEntrega
        };
    }
}

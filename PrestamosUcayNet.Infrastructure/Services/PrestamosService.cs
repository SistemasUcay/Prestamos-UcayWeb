using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de préstamos. Reside en Infrastructure para poder
/// usar EF Core Include() y cargar correctamente las entidades de navegación.
/// </summary>
public class PrestamosService : IPrestamosService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICalculadoraAmortizacionService _calculadora;

    public PrestamosService(
        UcayNetDbContext context,
        IMapper mapper,
        ICalculadoraAmortizacionService calculadora)
    {
        _context = context;
        _mapper = mapper;
        _calculadora = calculadora;
    }

    /// <inheritdoc />
    public async Task<int> CrearPrestamoAsync(PrestamoCreateDto dto)
    {
        var empleado = await _context.Empleados.FindAsync(dto.EmpleadoId)
            ?? throw new InvalidOperationException("El empleado no existe.");

        // Auto-crear solicitud asociada (SolicitudId es FK no-nullable)
        var solicitud = new SolicitudPrestamo
        {
            EmpleadoId = dto.EmpleadoId,
            EmpresaEmpleadoraId = empleado.EmpresaId,
            FondeadoraId = dto.FondeadoraId,
            CondicionCreditoId = dto.CondicionCreditoId ?? 0,
            MontoSolicitado = dto.MontoAprobado,
            PlazoMeses = dto.PlazoMeses,
            Motivo = dto.DestinoCredito,
            EstadoSolicitud = "Entregada",
            FechaSolicitud = DateTime.Now
        };

        _context.SolicitudesPrestamo.Add(solicitud);
        await _context.SaveChangesAsync();

        var nuevoPrestamo = new Prestamo
        {
            SolicitudId = solicitud.Id,
            EmpleadoId = dto.EmpleadoId,
            FuenteFondeoId = dto.FondeadoraId,
            TipoPrestamoId = dto.TipoPrestamoId,
            TipoPagoId = dto.TipoPagoId,
            MontoAprobado = dto.MontoAprobado,
            PlazoMeses = dto.PlazoMeses,
            TasaInteresMensual = dto.TasaInteresMensual,
            TasaInteresAnual = dto.TasaInteresMensual * 12,
            DestinoCredito = dto.DestinoCredito,
            FechaSolicitud = DateTime.Now,
            FechaAprobado = DateTime.Now,
            FechaDeposito = dto.FechaEntrega,
            MetodoEntrega = dto.MetodoEntrega,
            ReferenciaBancaria = dto.ReferenciaBancaria,
            Folio = dto.Folio,
            Estado = "A",
            SueldoLaboral = empleado.SueldoLaboral ?? 0,
            NumeroCuotas = dto.PlazoMeses,
            InicioDescuento = dto.FechaEntrega.AddMonths(1),
            FinalDescuento = dto.FechaEntrega.AddMonths(dto.PlazoMeses),
            AvalCredito = dto.EmpleadoIdAval.HasValue,
            EmpleadoIdaval = dto.EmpleadoIdAval ?? 0,
            TotalPagar = 0,
            SaldoPendiente = dto.MontoAprobado
        };

        _context.Prestamos.Add(nuevoPrestamo);
        await _context.SaveChangesAsync();

        await _calculadora.GenerarYGuardarTablaAmortizacionAsync(nuevoPrestamo.PrestamoId);

        return nuevoPrestamo.PrestamoId;
    }

    /// <inheritdoc />
    public async Task<PrestamoDetalleDto?> ObtenerDetalleAsync(int id)
    {
        var prestamo = await QueryConNavegacion()
            .Include(p => p.PrestamosAmortizaciones)
            .FirstOrDefaultAsync(p => p.PrestamoId == id);

        return _mapper.Map<PrestamoDetalleDto>(prestamo);
    }

    /// <inheritdoc />
    public async Task<List<PrestamoListDto>> ListarTodosAsync()
    {
        return await QueryConNavegacion()
            .OrderByDescending(p => p.FechaAprobado)
            .ProjectTo<PrestamoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<PrestamoListDto>> ListarPorEstadoAsync(string estado)
    {
        return await QueryConNavegacion()
            .Where(p => p.Estado == estado)
            .OrderByDescending(p => p.FechaAprobado)
            .ProjectTo<PrestamoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<PrestamoListDto>> ListarPorEmpleadoAsync(int empleadoId)
    {
        return await QueryConNavegacion()
            .Where(p => p.EmpleadoId == empleadoId)
            .OrderByDescending(p => p.FechaAprobado)
            .ProjectTo<PrestamoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ActivarPrestamoAsync(ActivarPrestamoDto dto)
    {
        var solicitud = await _context.SolicitudesPrestamo
            .Include(s => s.Empleado)
            .Include(s => s.CondicionCredito)
            .FirstOrDefaultAsync(s => s.Id == dto.SolicitudId);

        if (solicitud is null || solicitud.EstadoSolicitud != "Aprobada")
            throw new InvalidOperationException("La solicitud no está aprobada para ser activada.");

        var nuevoPrestamo = new Prestamo
        {
            SolicitudId = solicitud.Id,
            EmpleadoId = solicitud.EmpleadoId,
            FuenteFondeoId = solicitud.FondeadoraId,
            MontoAprobado = solicitud.MontoSolicitado,
            PlazoMeses = solicitud.PlazoMeses,
            TasaInteresMensual = solicitud.CondicionCredito.TasaInteresMensual,
            TasaInteresAnual = solicitud.CondicionCredito.TasaInteresMensual * 12,
            FechaSolicitud = solicitud.FechaSolicitud,
            FechaAprobado = DateTime.Now,
            FechaDeposito = dto.FechaEntrega,
            MetodoEntrega = dto.MetodoEntrega,
            ReferenciaBancaria = dto.Referencia,
            Folio = dto.Folio,
            Estado = "A",
            SueldoLaboral = solicitud.Empleado.SueldoLaboral ?? 0,
            TipoPrestamoId = 1,
            DestinoCredito = solicitud.Motivo,
            TipoPagoId = solicitud.Empleado.TipoPagoId,
            NumeroCuotas = solicitud.PlazoMeses,
            InicioDescuento = dto.FechaEntrega.AddMonths(1),
            FinalDescuento = dto.FechaEntrega.AddMonths(solicitud.PlazoMeses),
            TotalPagar = 0,
            SaldoPendiente = solicitud.MontoSolicitado
        };

        _context.Prestamos.Add(nuevoPrestamo);

        solicitud.EstadoSolicitud = "Entregada";
        await _context.SaveChangesAsync();

        await _calculadora.GenerarYGuardarTablaAmortizacionAsync(nuevoPrestamo.PrestamoId);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ActualizarPrestamoAsync(PrestamoUpdateDto dto)
    {
        var prestamo = await _context.Prestamos.FindAsync(dto.PrestamoId);
        if (prestamo is null) return false;

        if (dto.DestinoCredito is not null)
            prestamo.DestinoCredito = dto.DestinoCredito;

        if (dto.MetodoEntrega is not null)
            prestamo.MetodoEntrega = dto.MetodoEntrega;

        if (dto.ReferenciaBancaria is not null)
            prestamo.ReferenciaBancaria = dto.ReferenciaBancaria;

        if (dto.Folio is not null)
            prestamo.Folio = dto.Folio;

        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> CancelarPrestamoAsync(int prestamoId)
    {
        var prestamo = await _context.Prestamos.FindAsync(prestamoId);
        if (prestamo is null || prestamo.Estado != "A") return false;

        prestamo.Estado = "C";
        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> RegistrarPagoAsync(RegistrarPagoDto dto)
    {
        var prestamo = await _context.Prestamos.FindAsync(dto.PrestamoId);
        if (prestamo is null || prestamo.Estado != "A") return false;

        var nuevoPago = new Pago
        {
            PrestamoId = dto.PrestamoId,
            MontoPagado = dto.Monto,
            FechaPago = dto.FechaPago,
            TipoPago = dto.TipoPago,
            MetodoPago = dto.MetodoPago,
            Notas = dto.Referencia
        };

        _context.Pagos.Add(nuevoPago);

        prestamo.SaldoPendiente -= dto.Monto;
        if (prestamo.SaldoPendiente <= 0)
        {
            prestamo.SaldoPendiente = 0;
            prestamo.Estado = "P";
        }

        await _context.SaveChangesAsync();

        if (dto.TipoPago == "Extraordinario")
        {
            await _calculadora.RecalcularYGuardarPorPagoAnticipadoAsync(
                dto.PrestamoId,
                dto.Monto,
                !dto.ReducirPlazo);
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> SolicitarPausaAsync(SolicitudPausaDto dto)
    {
        var prestamo = await _context.Prestamos.FindAsync(dto.PrestamoId);
        if (prestamo is null || prestamo.Estado != "A") return false;

        await _calculadora.PausarCuotasAsync(dto.PrestamoId, dto.MesesPausa);

        var pausa = new PausaPrestamo
        {
            PrestamoId = dto.PrestamoId,
            FechaInicio = DateTime.Now,
            DuracionMeses = dto.MesesPausa,
            Motivo = dto.Motivo,
            FechaFin = DateTime.Now.AddMonths(dto.MesesPausa),
            Activa = true
        };

        _context.PausasPrestamo.Add(pausa);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public AmortizacionPreviewResultDto SimularAmortizacion(SimularAmortizacionDto dto)
    {
        return _calculadora.SimularAmortizacion(dto);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private IQueryable<Prestamo> QueryConNavegacion() =>
        _context.Prestamos
            .Include(p => p.Empleado)
            .Include(p => p.FuenteFondeo)
            .Include(p => p.TipoPrestamo)
            .Include(p => p.TipoPago)
            .AsNoTracking();
}

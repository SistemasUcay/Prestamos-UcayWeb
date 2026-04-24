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
                
        var nuevoPrestamo = new Prestamo
        {
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
        var prestamo = await _context.Prestamos
            .Include(p => p.Empleado)
            .FirstOrDefaultAsync(p => p.PrestamoId == dto.PrestamoId);

        if (prestamo is null || prestamo.Estado != "E")
            throw new InvalidOperationException("El préstamo no está en evaluación para ser activado.");

        prestamo.Estado = "A"; // Activo
        prestamo.FechaDeposito = dto.FechaEntrega;
        prestamo.MetodoEntrega = dto.MetodoEntrega;
        prestamo.ReferenciaBancaria = dto.Referencia;

        if (!string.IsNullOrEmpty(dto.Folio))
            prestamo.Folio = dto.Folio;

        prestamo.InicioDescuento = dto.FechaEntrega.AddMonths(1);
        prestamo.FinalDescuento = dto.FechaEntrega.AddMonths(prestamo.PlazoMeses);

        await _context.SaveChangesAsync();

        await _calculadora.GenerarYGuardarTablaAmortizacionAsync(prestamo.PrestamoId);

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

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de solicitudes de préstamo. Reside en Infrastructure para poder
/// usar EF Core Include() y cargar correctamente las entidades de navegación.
/// </summary>
public class SolicitudesService : ISolicitudesService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;
    private const decimal PorcentajeMaximoEndeudamiento = 0.30m;

    public SolicitudesService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<CapacidadPagoResultDto> EvaluarCapacidadDePagoAsync(CapacidadPagoDto dto)
    {
        var empleado = await _context.Empleados.FindAsync(dto.EmpleadoId)
            ?? throw new ArgumentException("Empleado no encontrado.");

        var condicion = await _context.CondicionesCredito.FindAsync(dto.CondicionCreditoId)
            ?? throw new ArgumentException("Condición de Crédito no encontrada.");

        decimal sueldo = empleado.SueldoLaboral ?? 0m;
        decimal cuotaEstimada = CalcularCuota(dto.MontoSolicitado, dto.PlazoMeses, condicion.TasaInteresMensual / 100m);

        decimal descuentosActivos = await _context.Prestamos
            .Where(p => p.EmpleadoId == dto.EmpleadoId && p.Estado == "A")
            .SumAsync(p => (decimal?)p.ImporteCuotas ?? 0m);

        decimal cuotaMaxPermitida = Math.Round(sueldo * PorcentajeMaximoEndeudamiento, 2);
        decimal capacidadDisponible = Math.Max(0, cuotaMaxPermitida - descuentosActivos);
        bool aprobado = cuotaEstimada <= capacidadDisponible;

        return new CapacidadPagoResultDto
        {
            SueldoNeto = sueldo,
            CuotaMaxPermitida = cuotaMaxPermitida,
            DescuentosActivos = descuentosActivos,
            CapacidadDisponible = capacidadDisponible,
            CuotaEstimadaProyectada = cuotaEstimada,
            EsAprobado = aprobado,
            Mensaje = aprobado
                ? "Capacidad de pago suficiente para el monto y plazo solicitados."
                : "La cuota proyectada supera la capacidad de pago disponible."
        };
    }

    /// <inheritdoc />
    public async Task<int> CrearSolicitudAsync(CrearSolicitudDto dto)
    {
        var condicion = await _context.CondicionesCredito.FindAsync(dto.CondicionCreditoId)
            ?? throw new ArgumentException("Condición de crédito inválida.");

        if (dto.MontoSolicitado < condicion.MontoMinimo || dto.MontoSolicitado > condicion.MontoMaximo)
            throw new InvalidOperationException(
                $"El monto solicitado rebasa los límites permitidos [{condicion.MontoMinimo:C} - {condicion.MontoMaximo:C}].");

        if (dto.PlazoMeses > condicion.PlazoMaximoMeses || dto.PlazoMeses <= 0)
            throw new InvalidOperationException(
                $"El plazo excede el máximo permitido ({condicion.PlazoMaximoMeses} meses).");

        var empleado = await _context.Empleados.FindAsync(dto.EmpleadoId)
            ?? throw new ArgumentException("Empleado no encontrado.");

        var eval = await EvaluarCapacidadDePagoAsync(new CapacidadPagoDto
        {
            CondicionCreditoId = dto.CondicionCreditoId,
            EmpleadoId = dto.EmpleadoId,
            FondeadoraId = condicion.FondeadoraId,
            MontoSolicitado = dto.MontoSolicitado,
            PlazoMeses = dto.PlazoMeses
        });

        if (!eval.EsAprobado)
            throw new InvalidOperationException(eval.Mensaje);

        var nuevaSolicitud = _mapper.Map<SolicitudPrestamo>(dto);
        nuevaSolicitud.EmpresaEmpleadoraId = empleado.EmpresaId;
        nuevaSolicitud.FondeadoraId = condicion.FondeadoraId;
        nuevaSolicitud.EstadoSolicitud = "Trámite";
        nuevaSolicitud.FechaSolicitud = DateTime.UtcNow;

        _context.SolicitudesPrestamo.Add(nuevaSolicitud);
        await _context.SaveChangesAsync();

        return nuevaSolicitud.Id;
    }

    /// <inheritdoc />
    public async Task<SolicitudDetalleDto?> ObtenerPorIdAsync(int id)
    {
        var sol = await QueryConNavegacion()
            .FirstOrDefaultAsync(s => s.Id == id);
        return _mapper.Map<SolicitudDetalleDto>(sol);
    }

    /// <inheritdoc />
    public async Task<List<SolicitudDetalleDto>> ListarTodasAsync()
    {
        var solicitudes = await QueryConNavegacion()
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return _mapper.Map<List<SolicitudDetalleDto>>(solicitudes);
    }

    /// <inheritdoc />
    public async Task<List<SolicitudDetalleDto>> ListarPendientesAsync()
        => await ListarPorEstadoAsync("Trámite");

    /// <inheritdoc />
    public async Task<List<SolicitudDetalleDto>> ListarPorEstadoAsync(string estado)
    {
        var solicitudes = await QueryConNavegacion()
            .Where(s => s.EstadoSolicitud == estado)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return _mapper.Map<List<SolicitudDetalleDto>>(solicitudes);
    }

    /// <inheritdoc />
    public async Task<List<SolicitudDetalleDto>> ListarPorEmpresaAsync(int empresaId)
    {
        var solicitudes = await QueryConNavegacion()
            .Where(s => s.EmpresaEmpleadoraId == empresaId)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return _mapper.Map<List<SolicitudDetalleDto>>(solicitudes);
    }

    /// <inheritdoc />
    public async Task<bool> AprobarSolicitudAsync(AprobarSolicitudDto dto)
    {
        var sol = await _context.SolicitudesPrestamo.FindAsync(dto.SolicitudId);
        if (sol == null || sol.EstadoSolicitud != "Trámite") return false;

        sol.EstadoSolicitud = "Aprobada";
        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> RechazarSolicitudAsync(RechazarSolicitudDto dto)
    {
        var sol = await _context.SolicitudesPrestamo.FindAsync(dto.SolicitudId);
        if (sol == null || sol.EstadoSolicitud != "Trámite") return false;

        sol.EstadoSolicitud = "Rechazada";
        sol.MotivoRechazo = dto.MotivoRechazo;
        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> CancelarSolicitudAsync(CancelarSolicitudDto dto)
    {
        var sol = await _context.SolicitudesPrestamo.FindAsync(dto.SolicitudId);
        if (sol == null || sol.EstadoSolicitud != "Trámite") return false;

        sol.EstadoSolicitud = "Cancelada";
        sol.MotivoCancelacion = dto.MotivoCancelacion;
        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> EditarSolicitudAsync(EditarSolicitudDto dto)
    {
        var sol = await _context.SolicitudesPrestamo.FindAsync(dto.SolicitudId);
        if (sol == null || sol.EstadoSolicitud != "Trámite") return false;

        var condicion = await _context.CondicionesCredito.FindAsync(dto.CondicionCreditoId)
            ?? throw new ArgumentException("Condición de crédito inválida.");

        if (dto.MontoSolicitado < condicion.MontoMinimo || dto.MontoSolicitado > condicion.MontoMaximo)
            throw new InvalidOperationException(
                $"El monto solicitado rebasa los límites permitidos [{condicion.MontoMinimo:C} - {condicion.MontoMaximo:C}].");

        if (dto.PlazoMeses > condicion.PlazoMaximoMeses || dto.PlazoMeses <= 0)
            throw new InvalidOperationException(
                $"El plazo excede el máximo permitido ({condicion.PlazoMaximoMeses} meses).");

        sol.CondicionCreditoId = dto.CondicionCreditoId;
        sol.FondeadoraId = condicion.FondeadoraId;
        sol.MontoSolicitado = dto.MontoSolicitado;
        sol.PlazoMeses = dto.PlazoMeses;
        sol.Motivo = dto.Motivo;

        return await _context.SaveChangesAsync() > 0;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private IQueryable<SolicitudPrestamo> QueryConNavegacion() =>
        _context.SolicitudesPrestamo
            .Include(s => s.Empleado)
            .Include(s => s.EmpresaEmpleadora)
            .Include(s => s.Fondeadora)
            .Include(s => s.CondicionCredito)
            .AsNoTracking();

    private static decimal CalcularCuota(decimal P, int n, decimal i)
    {
        if (i == 0) return Math.Round(P / n, 2);
        decimal factor = (decimal)Math.Pow((double)(1 + i), n);
        return Math.Round(P * (i * factor) / (factor - 1), 2);
    }
}

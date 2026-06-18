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

        // Validación de negocio: la solicitud debe tener al menos una modalidad válida.
        bool tieneNomina    = dto.MontoAprobado > 0;
        bool tieneDirectos  = dto.PrestamosDirectos?.Count > 0;

        if (!tieneNomina && !tieneDirectos)
            throw new InvalidOperationException("La solicitud debe incluir al menos un Préstamo Nómina o un Préstamo Directo.");

        if (tieneNomina && (dto.PlazoMeses <= 0 || dto.TasaInteresMensual <= 0))
            throw new InvalidOperationException("El Préstamo Nómina requiere plazo y tasa de interés mayores a cero.");

        // Calcular totales desde las amortizaciones enviadas
        decimal totalPagar = dto.Amortizaciones.Sum(a => a.ImportePago);
        decimal totalIntereses = dto.Amortizaciones.Sum(a => a.ImporteInteres);
        decimal totalSolicitud = dto.MontoAprobado + (dto.PrestamosDirectos?.Sum(pd => pd.MontoSolicitado) ?? 0);
        DateTime fechaUltimoPago = dto.Amortizaciones.Count > 0 
            ? dto.Amortizaciones.Max(a => a.FechaPago) 
            : dto.FechaEntrega;

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
            Estado = "E", // La Solicitud se crea con el Estatus [E] En Evaluación
            SueldoLaboral = empleado.SueldoLaboral ?? 0,
            NumeroCuotas = dto.Amortizaciones.Count,
            ImporteCuotas = dto.Amortizaciones.Count > 0 ? dto.Amortizaciones[0].ImportePago : 0,
            InicioDescuento = dto.FechaEntrega.AddMonths(1),
            FinalDescuento = fechaUltimoPago,
            AvalCredito = dto.EmpleadoIdAval.HasValue,
            EmpleadoIdaval = dto.EmpleadoIdAval ?? 0,
            TotalPagar = totalPagar,
            SaldoPendiente = dto.MontoAprobado,
            TotalSolicitud = totalSolicitud
        };

        _context.Prestamos.Add(nuevoPrestamo);
        await _context.SaveChangesAsync();

        // Guardar las amortizaciones pre-calculadas desde el frontend
        if (dto.Amortizaciones?.Count > 0)
        {
            var amortizaciones = dto.Amortizaciones.Select(a => new PrestamosAmortizacione
            {
                PrestamoId = nuevoPrestamo.PrestamoId,
                NumeroPago = a.NumeroPago,
                FechaPago = a.FechaPago,
                InicioPeriodoPago = a.FechaPago.AddMonths(-1),
                DescripcionPago = $"Cuota {a.NumeroPago}/{dto.Amortizaciones.Count}",
                TasaInteres = dto.TasaInteresMensual,
                SaldoInicial = a.NumeroPago > 1 
                    ? dto.Amortizaciones[a.NumeroPago - 2].SaldoFinal 
                    : dto.MontoAprobado,
                ImportePago = a.ImportePago,
                ImportePagoCapital = a.ImporteCapital,
                ImportePagoInteres = a.ImporteInteres,
                PagoExtraCapital = 0,
                SaldoFinal = a.SaldoFinal,
                StatusPago = false
            }).ToList();

            _context.PrestamosAmortizaciones.AddRange(amortizaciones);
            await _context.SaveChangesAsync();
        }

        // Guardar préstamos directos
        if (dto.PrestamosDirectos?.Count > 0)
        {
            var prestamosDirectos = dto.PrestamosDirectos.Select((pd, index) => new PrestamoDirecto
            {
                PrestamoId = nuevoPrestamo.PrestamoId,
                Descripcion = pd.Descripcion,
                MontoSolicitado = pd.MontoSolicitado,
                TasaMensual = pd.TasaMensual,
                FechaPago = pd.FechaPago,
                DiazPlazo = pd.DiazPlazo,
                MontoIntereses = Math.Round(pd.MontoSolicitado * (pd.TasaMensual / 100m) * (pd.DiazPlazo / 30m), 2),
                TotalCapitalInteres = pd.MontoSolicitado + Math.Round(pd.MontoSolicitado * (pd.TasaMensual / 100m) * (pd.DiazPlazo / 30m), 2)
            }).ToList();

            _context.PrestamosDirectos.AddRange(prestamosDirectos);
            await _context.SaveChangesAsync();
        }

        return nuevoPrestamo.PrestamoId;
    }

    /// <inheritdoc />
    public async Task<PrestamoDetalleDto?> ObtenerDetalleAsync(int id)
    {
        var prestamo = await QueryConNavegacion()
            .Include(p => p.PrestamosAmortizaciones)
            .Include(p => p.PrestamosDirectos)
            .FirstOrDefaultAsync(p => p.PrestamoId == id);

        return _mapper.Map<PrestamoDetalleDto>(prestamo);
    }

    /// <inheritdoc />
    public async Task<List<PrestamoListDto>> ListarTodosAsync()
    {
        return await QueryConNavegacion()
            .AsSplitQuery()
            .OrderByDescending(p => p.FechaAprobado)
            .ProjectTo<PrestamoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<PrestamoListDto>> ListarPorEstadoAsync(string estado)
    {
        return await QueryConNavegacion()
            .AsSplitQuery()
            .Where(p => p.Estado == estado)
            .OrderByDescending(p => p.FechaAprobado)
            .ProjectTo<PrestamoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<PrestamoListDto>> ListarPorEmpleadoAsync(int empleadoId)
    {
        return await QueryConNavegacion()
            .AsSplitQuery()
            .Where(p => p.EmpleadoId == empleadoId)
            .OrderByDescending(p => p.FechaAprobado)
            .ProjectTo<PrestamoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ActivarPrestamoAsync(ActivarPrestamoDto dto)
    {
        var prestamo = await _context.Prestamos.FindAsync(dto.PrestamoId);

        if (prestamo is null)
            throw new InvalidOperationException("El préstamo no existe.");

        if (prestamo.Estado != "T")
            throw new InvalidOperationException("Solo se pueden activar préstamos en estado 'Pendiente de Entrega'.");

        // Activación operativa: únicamente cambia el estado.
        // No se deben modificar datos de la solicitud ni regenerar tabla de amortización.
        prestamo.Estado = "A";

        return await _context.SaveChangesAsync() > 0;
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
    public async Task<bool> EvaluarPrestamoAsync(EvaluarPrestamoDto dto)
    {
        return dto.Accion?.ToUpper() switch
        {
            "APROBAR" => await AprobarPrestamoAsync(
                dto.PrestamoId,
                dto.FechaEntrega ?? DateTime.Now,
                dto.MetodoEntrega ?? "Transferencia",
                dto.Referencia ?? "Aprobado",
                dto.Folio,
                dto.Notas),
            "RECHAZAR" => await RechazarPrestamoAsync(dto.PrestamoId, dto.Notas ?? "Rechazado"),
            "CANCELAR" => await CancelarSolicitudAsync(dto.PrestamoId, dto.Notas ?? "Cancelado"),
            _ => throw new InvalidOperationException("Acción de evaluación no válida. Use: Aprobar, Rechazar o Cancelar.")
        };
    }

    /// <inheritdoc />
    public async Task<bool> AprobarPrestamoAsync(int prestamoId, DateTime fechaEntrega, string metodoEntrega, string referencia, string? folio, string? notas)
    {
        var prestamo = await _context.Prestamos
            .Include(p => p.Empleado)
            .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);

        if (prestamo is null)
            throw new InvalidOperationException("El préstamo no existe.");

        if (prestamo.Estado != "E")
            throw new InvalidOperationException("Solo se pueden aprobar préstamos en evaluación.");

        prestamo.Estado = "T"; // Pendiente de Entrega

        if (!string.IsNullOrEmpty(notas))
            prestamo.DestinoCredito = $"APROBADO: {notas}";

        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RechazarPrestamoAsync(int prestamoId, string notas)
    {
        ArgumentNullException.ThrowIfNull(notas);

        if (string.IsNullOrWhiteSpace(notas))
            throw new InvalidOperationException("Debe proporcionar el motivo del rechazo.");

        var prestamo = await _context.Prestamos.FindAsync(prestamoId);

        if (prestamo is null)
            throw new InvalidOperationException("El préstamo no existe.");

        if (prestamo.Estado != "E")
            throw new InvalidOperationException("Solo se pueden rechazar préstamos en evaluación.");

        prestamo.Estado = "R"; // Rechazado
        prestamo.DestinoCredito = $"RECHAZADO: {notas}";

        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> CancelarSolicitudAsync(int prestamoId, string notas)
    {
        ArgumentNullException.ThrowIfNull(notas);

        if (string.IsNullOrWhiteSpace(notas))
            throw new InvalidOperationException("Debe proporcionar el motivo de la cancelación.");

        var prestamo = await _context.Prestamos.FindAsync(prestamoId);

        if (prestamo is null)
            throw new InvalidOperationException("El préstamo no existe.");

        if (prestamo.Estado != "E")
            throw new InvalidOperationException("Solo se pueden cancelar préstamos en evaluación.");

        prestamo.Estado = "C"; // Cancelado
        prestamo.DestinoCredito = $"CANCELADO: {notas}";

        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DevolverAEvaluacionAsync(int prestamoId)
    {
        var prestamo = await _context.Prestamos.FindAsync(prestamoId);

        if (prestamo is null)
            throw new InvalidOperationException("El préstamo no existe.");

        if (prestamo.Estado != "T")
            throw new InvalidOperationException("Solo se pueden devolver a evaluación solicitudes en estado 'Pendiente de Entrega'.");

        prestamo.Estado = "E";
        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc />
    public AmortizacionPreviewResultDto SimularAmortizacion(SimularAmortizacionDto dto)
    {
        return _calculadora.SimularAmortizacion(dto);
    }

    /// <inheritdoc />
    public async Task<bool> ActualizarSolicitudAsync(int prestamoId, PrestamoCreateDto dto)
    {
        var prestamo = await _context.Prestamos.FindAsync(prestamoId);
        if (prestamo is null)
            throw new InvalidOperationException($"No se encontró la solicitud con ID {prestamoId}.");

        if (prestamo.Estado != "E")
            throw new InvalidOperationException("Solo se pueden modificar solicitudes en estado 'En Evaluación'.");

        var empleado = await _context.Empleados.FindAsync(dto.EmpleadoId)
            ?? throw new InvalidOperationException("El empleado no existe.");

        bool tieneNomina   = dto.MontoAprobado > 0;
        bool tieneDirectos = dto.PrestamosDirectos?.Count > 0;

        if (!tieneNomina && !tieneDirectos)
            throw new InvalidOperationException("La solicitud debe incluir al menos un Préstamo Nómina o un Préstamo Directo.");

        if (tieneNomina && (dto.PlazoMeses <= 0 || dto.TasaInteresMensual <= 0))
            throw new InvalidOperationException("El Préstamo Nómina requiere plazo y tasa de interés mayores a cero.");

        decimal totalPagar     = dto.Amortizaciones.Sum(a => a.ImportePago);
        decimal totalSolicitud = dto.MontoAprobado + (dto.PrestamosDirectos?.Sum(pd => pd.MontoSolicitado) ?? 0);
        DateTime fechaUltimoPago = dto.Amortizaciones.Count > 0
            ? dto.Amortizaciones.Max(a => a.FechaPago)
            : dto.FechaEntrega;

        // Actualizar campos del préstamo
        prestamo.EmpleadoId        = dto.EmpleadoId;
        prestamo.FuenteFondeoId    = dto.FondeadoraId;
        prestamo.TipoPrestamoId    = dto.TipoPrestamoId;
        prestamo.TipoPagoId        = dto.TipoPagoId;
        prestamo.MontoAprobado     = dto.MontoAprobado;
        prestamo.PlazoMeses        = dto.PlazoMeses;
        prestamo.TasaInteresMensual = dto.TasaInteresMensual;
        prestamo.TasaInteresAnual  = dto.TasaInteresMensual * 12;
        prestamo.DestinoCredito    = dto.DestinoCredito;
        prestamo.FechaSolicitud    = dto.FechaSolicitud;
        prestamo.FechaDeposito     = dto.FechaEntrega;
        prestamo.MetodoEntrega     = dto.MetodoEntrega;
        prestamo.ReferenciaBancaria = dto.ReferenciaBancaria;
        prestamo.Folio             = dto.Folio;
        prestamo.SueldoLaboral     = empleado.SueldoLaboral ?? 0;
        prestamo.NumeroCuotas      = dto.Amortizaciones.Count;
        prestamo.ImporteCuotas     = dto.Amortizaciones.Count > 0 ? dto.Amortizaciones[0].ImportePago : 0;
        prestamo.InicioDescuento   = dto.FechaEntrega.AddMonths(1);
        prestamo.FinalDescuento    = fechaUltimoPago;
        prestamo.AvalCredito       = dto.EmpleadoIdAval.HasValue;
        prestamo.EmpleadoIdaval    = dto.EmpleadoIdAval ?? 0;
        prestamo.TotalPagar        = totalPagar;
        prestamo.SaldoPendiente    = dto.MontoAprobado;
        prestamo.TotalSolicitud    = totalSolicitud;

        // Reemplazar amortizaciones
        var amortizacionesViejas = await _context.PrestamosAmortizaciones
            .Where(a => a.PrestamoId == prestamoId)
            .ToListAsync();
        _context.PrestamosAmortizaciones.RemoveRange(amortizacionesViejas);

        if (dto.Amortizaciones?.Count > 0)
        {
            var nuevasAmortizaciones = dto.Amortizaciones.Select(a => new PrestamosAmortizacione
            {
                PrestamoId           = prestamoId,
                NumeroPago           = a.NumeroPago,
                FechaPago            = a.FechaPago,
                InicioPeriodoPago    = a.FechaPago.AddMonths(-1),
                DescripcionPago      = $"Cuota {a.NumeroPago}/{dto.Amortizaciones.Count}",
                TasaInteres          = dto.TasaInteresMensual,
                SaldoInicial         = a.NumeroPago > 1
                                         ? dto.Amortizaciones[a.NumeroPago - 2].SaldoFinal
                                         : dto.MontoAprobado,
                ImportePago          = a.ImportePago,
                ImportePagoCapital   = a.ImporteCapital,
                ImportePagoInteres   = a.ImporteInteres,
                PagoExtraCapital     = 0,
                SaldoFinal           = a.SaldoFinal,
                StatusPago           = false
            }).ToList();
            _context.PrestamosAmortizaciones.AddRange(nuevasAmortizaciones);
        }

        // Reemplazar préstamos directos
        var directosViejos = await _context.PrestamosDirectos
            .Where(d => d.PrestamoId == prestamoId)
            .ToListAsync();
        _context.PrestamosDirectos.RemoveRange(directosViejos);

        if (dto.PrestamosDirectos?.Count > 0)
        {
            var nuevosDirectos = dto.PrestamosDirectos.Select(pd => new PrestamoDirecto
            {
                PrestamoId           = prestamoId,
                Descripcion          = pd.Descripcion,
                MontoSolicitado      = pd.MontoSolicitado,
                TasaMensual          = pd.TasaMensual,
                FechaPago            = pd.FechaPago,
                DiazPlazo            = pd.DiazPlazo,
                MontoIntereses       = Math.Round(pd.MontoSolicitado * (pd.TasaMensual / 100m) * (pd.DiazPlazo / 30m), 2),
                TotalCapitalInteres  = pd.MontoSolicitado + Math.Round(pd.MontoSolicitado * (pd.TasaMensual / 100m) * (pd.DiazPlazo / 30m), 2)
            }).ToList();
            _context.PrestamosDirectos.AddRange(nuevosDirectos);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private IQueryable<Prestamo> QueryConNavegacion() =>
        _context.Prestamos
            .Include(p => p.Empleado)
            .Include(p => p.FuenteFondeo)
            .Include(p => p.TipoPrestamo)
            .Include(p => p.TipoPago)
            .Include(p => p.PrestamosDirectos)
            .AsNoTracking();
}

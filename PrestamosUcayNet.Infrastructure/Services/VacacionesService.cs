using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

/// <summary>
/// Implementación de la lógica de negocio y persistencia para el Control de Vacaciones.
/// </summary>
public class VacacionesService : IVacacionesService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;

    public VacacionesService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResumenVacacionesEmpleadoDto?> ObtenerResumenPorEmpleadoAsync(
        int empleadoId,
        CancellationToken cancellationToken = default)
    {
        await AsegurarPeriodosGeneradosAsync(empleadoId, cancellationToken);

        var empleado = await _context.Empleados
            .AsNoTracking()
            .Include(e => e.Empresa)
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .FirstOrDefaultAsync(e => e.EmpleadoId == empleadoId, cancellationToken);

        if (empleado == null) return null;

        var periodos = await _context.PeriodosVacacionales
            .AsNoTracking()
            .Where(p => p.EmpleadoId == empleadoId)
            .OrderByDescending(p => p.Anio)
            .ProjectTo<PeriodoVacacionalDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var solicitudes = await _context.SolicitudesVacaciones
            .AsNoTracking()
            .Where(s => s.EmpleadoId == empleadoId)
            .OrderByDescending(s => s.FechaSolicitud)
            .Take(10)
            .ProjectTo<SolicitudVacacionesListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        int antiguedadAnios = 0;
        if (empleado.FechaIngreso.HasValue)
        {
            var hoy = DateTime.Today;
            antiguedadAnios = hoy.Year - empleado.FechaIngreso.Value.Year;
            if (empleado.FechaIngreso.Value.Date > hoy.AddYears(-antiguedadAnios))
                antiguedadAnios--;
            if (antiguedadAnios < 0) antiguedadAnios = 0;
        }

        int diasLeyPeriodoActual = CalcularDiasPorAntiguedad(antiguedadAnios);
        var periodoActivo = periodos.FirstOrDefault(p => p.Estado == "Activo")
                         ?? periodos.OrderByDescending(p => p.AntiguedadAnios).FirstOrDefault();

        int anioPeriodoActual = periodoActivo?.Anio ?? DateTime.Today.Year;
        int totalDiasDerecho = periodos.Sum(p => p.DiasCorrespondientes);
        int totalDiasDisfrutados = periodos.Sum(p => p.DiasDisfrutados);
        int totalDiasPendientes = periodos.Where(p => p.Estado == "Activo").Sum(p => p.DiasPendientes);
        int totalDiasCancelados = periodos.Where(p => p.Estado == "Vencido").Sum(p => Math.Max(0, p.DiasCorrespondientes - p.DiasDisfrutados - p.DiasPendientes));

        return new ResumenVacacionesEmpleadoDto
        {
            EmpleadoId = empleado.EmpleadoId,
            EmpleadoNombre = $"{empleado.Nombres} {empleado.ApellidoPaterno} {empleado.ApellidoMaterno}".Trim(),
            NumeroNomina = empleado.NumeroNomina,
            Rfc = empleado.Rfc,
            EmpresaNombre = empleado.Empresa?.RazonSocial ?? string.Empty,
            DepartamentoNombre = empleado.Departamento?.Descripcion ?? string.Empty,
            PuestoNombre = empleado.Puesto?.Descripcion ?? string.Empty,
            FechaIngreso = empleado.FechaIngreso,
            AntiguedadAnios = antiguedadAnios,
            DiasLeyPeriodoActual = diasLeyPeriodoActual,
            AnioPeriodoActual = anioPeriodoActual,
            FechaInicioPeriodoGenerador = periodoActivo?.FechaInicioPeriodo,
            FechaFinPeriodoGenerador = periodoActivo?.FechaFinPeriodo,
            FechaInicioPeriodoGoce = periodoActivo?.FechaInicioGoce,
            FechaFinPeriodoGoce = periodoActivo?.FechaFinGoce,
            FechaCancelacionPeriodoActual = periodoActivo?.FechaCancelacion,
            TotalDiasDerecho = totalDiasDerecho,
            TotalDiasDisfrutados = totalDiasDisfrutados,
            TotalDiasPendientes = totalDiasPendientes,
            TotalDiasCancelados = totalDiasCancelados,
            Periodos = periodos,
            SolicitudesRecientes = solicitudes
        };
    }

    public async Task<IEnumerable<PeriodoVacacionalDto>> ObtenerPeriodosPorEmpleadoAsync(
        int empleadoId,
        CancellationToken cancellationToken = default)
    {
        await AsegurarPeriodosGeneradosAsync(empleadoId, cancellationToken);

        return await _context.PeriodosVacacionales
            .AsNoTracking()
            .Where(p => p.EmpleadoId == empleadoId)
            .OrderByDescending(p => p.Anio)
            .ProjectTo<PeriodoVacacionalDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SolicitudVacacionesListDto>> ObtenerSolicitudesAsync(
        int? empleadoId = null,
        int? empresaId = null,
        string? estado = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SolicitudesVacaciones
            .AsNoTracking()
            .Include(s => s.Empleado)
                .ThenInclude(e => e.Empresa)
            .Include(s => s.Empleado)
                .ThenInclude(e => e.Departamento)
            .Include(s => s.Empleado)
                .ThenInclude(e => e.Puesto)
            .Include(s => s.PeriodoVacacional)
            .AsQueryable();

        if (empleadoId.HasValue)
            query = query.Where(s => s.EmpleadoId == empleadoId.Value);

        if (empresaId.HasValue)
            query = query.Where(s => s.Empleado.EmpresaId == empresaId.Value);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(s => s.Estado == estado.Trim());

        return await query
            .OrderByDescending(s => s.FechaSolicitud)
            .ProjectTo<SolicitudVacacionesListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<SolicitudVacacionesDetailDto?> ObtenerSolicitudPorIdAsync(
        int solicitudId,
        CancellationToken cancellationToken = default)
    {
        var solicitud = await _context.SolicitudesVacaciones
            .AsNoTracking()
            .Include(s => s.Empleado)
                .ThenInclude(e => e.Empresa)
            .Include(s => s.Empleado)
                .ThenInclude(e => e.Departamento)
            .Include(s => s.Empleado)
                .ThenInclude(e => e.Puesto)
            .Include(s => s.PeriodoVacacional)
            .FirstOrDefaultAsync(s => s.SolicitudVacacionesId == solicitudId, cancellationToken);

        if (solicitud == null) return null;

        return _mapper.Map<SolicitudVacacionesDetailDto>(solicitud);
    }

    public async Task<(bool Success, string? Error, int Id)> CrearSolicitudAsync(
        CrearSolicitudVacacionesDto dto,
        string usuarioCreacion,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (dto.FechaFin.Date < dto.FechaInicio.Date)
                return (false, "La fecha de fin no puede ser anterior a la fecha de inicio.", 0);

            var empleado = await _context.Empleados.FindAsync(new object[] { dto.EmpleadoId }, cancellationToken);
            if (empleado == null)
                return (false, "El empleado especificado no existe.", 0);

            await AsegurarPeriodosGeneradosAsync(dto.EmpleadoId, cancellationToken);

            int diasSolicitados = dto.DiasSolicitados ?? ((dto.FechaFin.Date - dto.FechaInicio.Date).Days + 1);
            if (diasSolicitados <= 0)
                return (false, "La cantidad de días solicitados debe ser mayor a cero.", 0);

            // Siempre se afecta el periodo vigente (generado por el último año de servicio cumplido y en periodo de goce actual)
            PeriodoVacacional? periodo = null;
            if (dto.PeriodoVacacionalId.HasValue && dto.PeriodoVacacionalId.Value > 0)
            {
                periodo = await _context.PeriodosVacacionales
                    .FirstOrDefaultAsync(p => p.PeriodoVacacionalId == dto.PeriodoVacacionalId.Value && p.EmpleadoId == dto.EmpleadoId, cancellationToken);

                if (periodo == null)
                    return (false, "El periodo vacacional especificado no existe o no pertenece al empleado.", 0);

                if (periodo.Estado == "Vencido")
                    return (false, $"El periodo {periodo.Anio} ({periodo.FechaInicioPeriodo:dd/MM/yyyy} - {periodo.FechaFinPeriodo:dd/MM/yyyy}) ya se encuentra vencido/cancelado.", 0);

                if (periodo.DiasPendientes < diasSolicitados)
                    return (false, $"El periodo vigente ({periodo.FechaInicioPeriodo:dd/MM/yyyy} al {periodo.FechaFinPeriodo:dd/MM/yyyy}) solo cuenta con {periodo.DiasPendientes} día(s) pendientes. No es posible solicitar {diasSolicitados} día(s).", 0);
            }
            else
            {
                periodo = await _context.PeriodosVacacionales
                    .Where(p => p.EmpleadoId == dto.EmpleadoId && p.Estado == "Activo")
                    .OrderByDescending(p => p.Anio)
                    .FirstOrDefaultAsync(cancellationToken);

                if (periodo == null)
                {
                    return (false, "El empleado aún no cumple el primer año de servicio a partir de su fecha de ingreso o no cuenta con un periodo vacacional vigente para disfrutar.", 0);
                }

                if (periodo.DiasPendientes < diasSolicitados)
                {
                    return (false, $"El empleado solo cuenta con {periodo.DiasPendientes} día(s) disponibles del periodo generador {periodo.FechaInicioPeriodo:dd/MM/yyyy} al {periodo.FechaFinPeriodo:dd/MM/yyyy} (Solicitados: {diasSolicitados} días).", 0);
                }
            }

            var nuevaSolicitud = new SolicitudVacaciones
            {
                EmpleadoId = dto.EmpleadoId,
                PeriodoVacacionalId = periodo.PeriodoVacacionalId,
                FechaSolicitud = DateTime.Now,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                FechaRegreso = dto.FechaRegreso,
                DiasSolicitados = diasSolicitados,
                Estado = "Pendiente",
                Observaciones = dto.Observaciones,
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = string.IsNullOrWhiteSpace(usuarioCreacion) ? "Sistema" : usuarioCreacion
            };

            _context.SolicitudesVacaciones.Add(nuevaSolicitud);
            await _context.SaveChangesAsync(cancellationToken);

            return (true, null, nuevaSolicitud.SolicitudVacacionesId);
        }
        catch (Exception ex)
        {
            return (false, $"Error al crear la solicitud de vacaciones: {ex.Message}", 0);
        }
    }

    public async Task<(bool Success, string? Error)> ResolverSolicitudAsync(
        int solicitudId,
        AprobarRechazarSolicitudDto dto,
        string usuarioAprobador,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var solicitud = await _context.SolicitudesVacaciones
                .Include(s => s.PeriodoVacacional)
                .FirstOrDefaultAsync(s => s.SolicitudVacacionesId == solicitudId, cancellationToken);

            if (solicitud == null)
                return (false, "Solicitud de vacaciones no encontrada.");

            if (solicitud.Estado != "Pendiente")
                return (false, $"La solicitud ya se encuentra en estado '{solicitud.Estado}' y no puede ser modificada.");

            var periodo = solicitud.PeriodoVacacional;
            if (periodo == null)
                return (false, "El periodo vacacional asociado a la solicitud no fue encontrado.");

            var ahora = DateTime.Now;
            var usuario = string.IsNullOrWhiteSpace(usuarioAprobador) ? "Sistema" : usuarioAprobador;

            if (dto.Aprobada)
            {
                if (periodo.DiasPendientes < solicitud.DiasSolicitados)
                    return (false, $"El periodo {periodo.Anio} no cuenta con suficientes días pendientes ({periodo.DiasPendientes} disponibles vs {solicitud.DiasSolicitados} solicitados).");

                // Afectación de saldos del periodo
                periodo.DiasDisfrutados += solicitud.DiasSolicitados;
                periodo.DiasPendientes = periodo.DiasCorrespondientes - periodo.DiasDisfrutados;
                periodo.FechaModificacion = ahora;
                periodo.UsuarioModificacion = usuario;

                if (periodo.DiasPendientes == 0)
                    periodo.Estado = "Cerrado";

                solicitud.Estado = "Aprobada";
            }
            else
            {
                solicitud.Estado = "Rechazada";
            }

            solicitud.FechaAprobacion = ahora;
            solicitud.UsuarioAprobador = usuario;
            solicitud.ObservacionesAprobacion = dto.Observaciones;
            solicitud.FechaModificacion = ahora;
            solicitud.UsuarioModificacion = usuario;

            await _context.SaveChangesAsync(cancellationToken);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al resolver la solicitud: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> CancelarSolicitudAsync(
        int solicitudId,
        string usuario,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var solicitud = await _context.SolicitudesVacaciones
                .Include(s => s.PeriodoVacacional)
                .FirstOrDefaultAsync(s => s.SolicitudVacacionesId == solicitudId, cancellationToken);

            if (solicitud == null)
                return (false, "Solicitud no encontrada.");

            if (solicitud.Estado == "Cancelada" || solicitud.Estado == "Rechazada")
                return (false, $"La solicitud ya está en estado '{solicitud.Estado}'.");

            var ahora = DateTime.Now;
            var usuarioModif = string.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario;

            // Si la solicitud estaba aprobada, restituimos los días al periodo vacacional
            if (solicitud.Estado == "Aprobada" && solicitud.PeriodoVacacional != null)
            {
                var periodo = solicitud.PeriodoVacacional;
                periodo.DiasDisfrutados = Math.Max(0, periodo.DiasDisfrutados - solicitud.DiasSolicitados);
                periodo.DiasPendientes = periodo.DiasCorrespondientes - periodo.DiasDisfrutados;
                periodo.Estado = "Activo";
                periodo.FechaModificacion = ahora;
                periodo.UsuarioModificacion = usuarioModif;
            }

            solicitud.Estado = "Cancelada";
            solicitud.FechaModificacion = ahora;
            solicitud.UsuarioModificacion = usuarioModif;

            await _context.SaveChangesAsync(cancellationToken);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al cancelar la solicitud: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> GenerarPeriodosEmpleadoAsync(
        int empleadoId,
        string usuario,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(new object[] { empleadoId }, cancellationToken);
            if (empleado == null)
                return (false, "Empleado no encontrado.");

            if (!empleado.FechaIngreso.HasValue)
                return (false, "El empleado no cuenta con Fecha de Ingreso registrada para calcular su antigüedad.");

            var fechaIngreso = empleado.FechaIngreso.Value.Date;
            var hoy = DateTime.Today;

            if (fechaIngreso > hoy)
                return (false, "La fecha de ingreso del empleado es posterior a la fecha actual.");

            // Calcular años de antigüedad cumplidos a partir de la fecha de ingreso
            int antiguedadTotal = hoy.Year - fechaIngreso.Year;
            if (fechaIngreso > hoy.AddYears(-antiguedadTotal))
                antiguedadTotal--;

            // La regla entra en vigor únicamente a partir de que el empleado cumple su primer año de servicio
            if (antiguedadTotal < 1)
            {
                await AplicarReglaCaducidadInternaAsync(empleadoId, usuario, cancellationToken);
                return (false, $"El empleado ingresó el {fechaIngreso:dd/MM/yyyy} y aún no cumple su primer año de servicio ({fechaIngreso.AddYears(1):dd/MM/yyyy}) para generar derecho a vacaciones.");
            }

            var periodosExistentes = await _context.PeriodosVacacionales
                .Where(p => p.EmpleadoId == empleadoId)
                .ToListAsync(cancellationToken);

            var usuarioGen = string.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario;
            var ahora = DateTime.Now;
            int nuevosPeriodos = 0;

            for (int anioServicio = 1; anioServicio <= antiguedadTotal; anioServicio++)
            {
                // Periodo que genera las vacaciones (Año Anterior al goce)
                var fechaInicioGenerador = fechaIngreso.AddYears(anioServicio - 1);
                var fechaFinGenerador = fechaIngreso.AddYears(anioServicio).AddDays(-1);
                // Periodo para disfrutar (Goce): inicia al día siguiente de FechaFinGenerador y termina 1 año después menos 1 día
                var fechaFinGoce = fechaIngreso.AddYears(anioServicio + 1).AddDays(-1);
                int anioPeriodo = fechaFinGenerador.Year;

                if (periodosExistentes.Any(p => p.AntiguedadAnios == anioServicio || p.Anio == anioPeriodo))
                    continue;

                int diasCorrespondientes = CalcularDiasPorAntiguedad(anioServicio);

                var periodo = new PeriodoVacacional
                {
                    EmpleadoId = empleadoId,
                    Anio = anioPeriodo,
                    AntiguedadAnios = anioServicio,
                    DiasCorrespondientes = diasCorrespondientes,
                    DiasDisfrutados = 0,
                    DiasPendientes = diasCorrespondientes,
                    FechaInicioPeriodo = fechaInicioGenerador,
                    FechaFinPeriodo = fechaFinGenerador,
                    FechaVencimiento = fechaFinGoce, // Fecha límite inclusive para disfrutar; se cancela el día FechaVencimiento.AddDays(1)
                    Estado = "Activo",
                    FechaCreacion = ahora,
                    UsuarioCreacion = usuarioGen
                };

                _context.PeriodosVacacionales.Add(periodo);
                nuevosPeriodos++;
            }

            if (nuevosPeriodos > 0)
                await _context.SaveChangesAsync(cancellationToken);

            // Regla de negocio: Cancelar días pendientes cuyos periodos de goce hayan concluido según la Fecha de Ingreso
            await AplicarReglaCaducidadInternaAsync(empleadoId, usuarioGen, cancellationToken);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al generar periodos de vacaciones: {ex.Message}");
        }
    }

    /// <summary>
    /// Aplica la regla de caducidad anual: si al cumplirse el año de goce (calculado desde la Fecha de Ingreso)
    /// el empleado aún tiene días pendientes, se cancelan automáticamente al iniciar el nuevo periodo de goce.
    /// </summary>
    public async Task<(bool Success, string? Error, int PeriodosAfectados)> AplicarReglaCaducidadAnualAsync(
        int? empleadoId = null,
        string usuario = "Sistema",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var usuarioModif = string.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario;
            int totalAfectados = 0;

            if (empleadoId.HasValue)
            {
                totalAfectados = await AplicarReglaCaducidadInternaAsync(empleadoId.Value, usuarioModif, cancellationToken);
            }
            else
            {
                var empleadosIds = await _context.PeriodosVacacionales
                    .Select(p => p.EmpleadoId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                foreach (var empId in empleadosIds)
                {
                    totalAfectados += await AplicarReglaCaducidadInternaAsync(empId, usuarioModif, cancellationToken);
                }
            }

            return (true, null, totalAfectados);
        }
        catch (Exception ex)
        {
            return (false, $"Error al aplicar regla de caducidad anual: {ex.Message}", 0);
        }
    }

    /// <summary>
    /// Evalúa los periodos de un empleado tomando en cuenta su Fecha de Ingreso:
    /// - La regla entra en vigor únicamente cuando el empleado cumple al menos 1 año desde su Fecha de Ingreso.
    /// - Periodo generador (año anterior): [FechaIngreso + (k-1) años] al [FechaIngreso + k años - 1 día].
    /// - Periodo de goce y disfrute (año en curso): [FechaIngreso + k años] al [FechaIngreso + (k+1) años - 1 día].
    /// - Fecha de cancelación automática: [FechaIngreso + (k+1) años] (cuando hoy > FechaFinGoce).
    /// </summary>
    private async Task<int> AplicarReglaCaducidadInternaAsync(int empleadoId, string usuario, CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmpleadoId == empleadoId, cancellationToken);

        if (empleado == null || !empleado.FechaIngreso.HasValue)
            return 0;

        var fechaIngreso = empleado.FechaIngreso.Value.Date;
        var hoy = DateTime.Today;

        int antiguedadTotal = hoy.Year - fechaIngreso.Year;
        if (fechaIngreso > hoy.AddYears(-antiguedadTotal))
            antiguedadTotal--;

        var periodos = await _context.PeriodosVacacionales
            .Include(p => p.SolicitudesVacaciones)
            .Where(p => p.EmpleadoId == empleadoId)
            .OrderBy(p => p.AntiguedadAnios)
            .ThenBy(p => p.Anio)
            .ToListAsync(cancellationToken);

        if (periodos.Count == 0)
            return 0;

        var ahora = DateTime.Now;
        int afectados = 0;
        bool guardar = false;

        // Si el empleado aún no cumple 1 año desde su fecha de ingreso, no debe tener periodos devengados activos
        if (antiguedadTotal < 1)
        {
            foreach (var p in periodos)
            {
                if (p.SolicitudesVacaciones == null || p.SolicitudesVacaciones.Count == 0)
                {
                    _context.PeriodosVacacionales.Remove(p);
                    guardar = true;
                }
            }
            if (guardar)
                await _context.SaveChangesAsync(cancellationToken);
            return 0;
        }

        foreach (var periodo in periodos)
        {
            int anioServicio = periodo.AntiguedadAnios > 0
                ? periodo.AntiguedadAnios
                : Math.Max(1, periodo.Anio - fechaIngreso.Year);

            // Fechas exactas basadas en FechaIngreso y el año de servicio cumplido
            var fechaInicioGenerador = fechaIngreso.AddYears(anioServicio - 1);
            var fechaFinGenerador = fechaIngreso.AddYears(anioServicio).AddDays(-1);
            var fechaFinGoce = fechaIngreso.AddYears(anioServicio + 1).AddDays(-1);
            int anioEsperado = fechaFinGenerador.Year;

            if (periodo.FechaInicioPeriodo.Date != fechaInicioGenerador ||
                periodo.FechaFinPeriodo.Date != fechaFinGenerador ||
                !periodo.FechaVencimiento.HasValue ||
                periodo.FechaVencimiento.Value.Date != fechaFinGoce ||
                periodo.AntiguedadAnios != anioServicio)
            {
                periodo.AntiguedadAnios = anioServicio;
                periodo.FechaInicioPeriodo = fechaInicioGenerador;
                periodo.FechaFinPeriodo = fechaFinGenerador;
                periodo.FechaVencimiento = fechaFinGoce;
                if (!periodos.Any(o => o.PeriodoVacacionalId != periodo.PeriodoVacacionalId && o.Anio == anioEsperado))
                {
                    periodo.Anio = anioEsperado;
                }
                guardar = true;
            }

            // El empleado tiene hasta fechaFinGoce (inclusive) para disfrutar sus días.
            // Al día siguiente (hoy.Date > fechaFinGoce, cuando inicia el nuevo periodo de goce) se cancelan automáticamente.
            bool debeCaducar = hoy.Date > fechaFinGoce || anioServicio < antiguedadTotal;

            if (debeCaducar)
            {
                if (periodo.DiasPendientes > 0 || periodo.Estado == "Activo")
                {
                    periodo.DiasPendientes = 0;
                    periodo.Estado = "Vencido";
                    periodo.FechaModificacion = ahora;
                    periodo.UsuarioModificacion = string.IsNullOrWhiteSpace(usuario) ? "Sistema (Cancelación Anual)" : usuario;
                    guardar = true;
                    afectados++;
                }
            }
            else if (anioServicio == antiguedadTotal)
            {
                // Es el periodo vigente (generado por el año anterior cumplido, con goce en el año en curso)
                int saldoCalculado = Math.Max(0, periodo.DiasCorrespondientes - periodo.DiasDisfrutados);
                string estadoEsperado = saldoCalculado == 0 ? "Cerrado" : "Activo";
                if (periodo.Estado != estadoEsperado || periodo.DiasPendientes != saldoCalculado)
                {
                    periodo.Estado = estadoEsperado;
                    periodo.DiasPendientes = saldoCalculado;
                    periodo.FechaModificacion = ahora;
                    periodo.UsuarioModificacion = usuario;
                    guardar = true;
                }
            }
        }

        if (guardar)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return afectados;
    }

    /// <summary>
    /// Calcula los días correspondientes según la tabla de vacaciones laborales (Ley Federal del Trabajo - Vacaciones Dignas).
    /// Si el empleado tiene menos de 1 año cumplido, aún no genera días (0).
    /// </summary>
    private static int CalcularDiasPorAntiguedad(int antiguedadAnios)
    {
        return antiguedadAnios switch
        {
            <= 0 => 0,
            1 => 12,
            2 => 14,
            3 => 16,
            4 => 18,
            5 => 20,
            >= 6 and <= 10 => 22,
            >= 11 and <= 15 => 24,
            >= 16 and <= 20 => 26,
            >= 21 and <= 25 => 28,
            >= 26 and <= 30 => 30,
            >= 31 and <= 35 => 32,
            _ => 32
        };
    }

    /// <summary>
    /// Verifica y genera automáticamente los periodos devengados por el empleado en base a su Fecha de Ingreso.
    /// Solo genera periodos a partir de que el empleado ha cumplido al menos 1 año de servicio.
    /// </summary>
    private async Task AsegurarPeriodosGeneradosAsync(int empleadoId, CancellationToken cancellationToken)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(new object[] { empleadoId }, cancellationToken);
            if (empleado == null || !empleado.FechaIngreso.HasValue)
                return;

            var fechaIngreso = empleado.FechaIngreso.Value.Date;
            var hoy = DateTime.Today;

            if (fechaIngreso > hoy)
                return;

            // Calcular antigüedad en años cumplidos a partir de la Fecha de Ingreso
            int antiguedadTotal = hoy.Year - fechaIngreso.Year;
            if (fechaIngreso > hoy.AddYears(-antiguedadTotal))
                antiguedadTotal--;

            // La regla solo entra en vigor a partir del primer año cumplido desde la fecha de ingreso
            if (antiguedadTotal < 1)
            {
                await AplicarReglaCaducidadInternaAsync(empleadoId, "Sistema", cancellationToken);
                return;
            }

            var periodosExistentes = await _context.PeriodosVacacionales
                .Where(p => p.EmpleadoId == empleadoId)
                .Select(p => new { p.Anio, p.AntiguedadAnios })
                .ToListAsync(cancellationToken);

            var ahora = DateTime.Now;
            bool nuevos = false;

            for (int anioServicio = 1; anioServicio <= antiguedadTotal; anioServicio++)
            {
                var fechaInicioGenerador = fechaIngreso.AddYears(anioServicio - 1);
                var fechaFinGenerador = fechaIngreso.AddYears(anioServicio).AddDays(-1);
                var fechaFinGoce = fechaIngreso.AddYears(anioServicio + 1).AddDays(-1);
                int anioPeriodo = fechaFinGenerador.Year;

                if (periodosExistentes.Any(p => p.AntiguedadAnios == anioServicio || p.Anio == anioPeriodo))
                    continue;

                int diasCorrespondientes = CalcularDiasPorAntiguedad(anioServicio);

                var periodo = new PeriodoVacacional
                {
                    EmpleadoId = empleadoId,
                    Anio = anioPeriodo,
                    AntiguedadAnios = anioServicio,
                    DiasCorrespondientes = diasCorrespondientes,
                    DiasDisfrutados = 0,
                    DiasPendientes = diasCorrespondientes,
                    FechaInicioPeriodo = fechaInicioGenerador,
                    FechaFinPeriodo = fechaFinGenerador,
                    FechaVencimiento = fechaFinGoce,
                    Estado = "Activo",
                    FechaCreacion = ahora,
                    UsuarioCreacion = "Sistema"
                };

                _context.PeriodosVacacionales.Add(periodo);
                periodosExistentes.Add(new { Anio = anioPeriodo, AntiguedadAnios = anioServicio });
                nuevos = true;
            }

            if (nuevos)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Aplicar regla de caducidad automática y normalización de fechas
            await AplicarReglaCaducidadInternaAsync(empleadoId, "Sistema", cancellationToken);
        }
        catch
        {
            // Evitar interrumpir la lectura si ocurre un bloqueo de concurrencia
        }
    }
}

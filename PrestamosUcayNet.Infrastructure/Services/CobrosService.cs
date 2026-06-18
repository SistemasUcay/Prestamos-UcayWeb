using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.Cobros.Commands;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

public class CobrosService : ICobrosService
{
    private readonly UcayNetDbContext _context;

    public CobrosService(UcayNetDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<AmortizacionPendienteDto>> GetAmortizacionesPendientesAsync(
        int? empresaId,
        int? tipoPagoId,
        DateTime? fechaVencimiento)
    {
        var amortizacionesQuery = _context.PrestamosAmortizaciones
            .Include(a => a.Prestamo)
                .ThenInclude(p => p.Empleado)
                    .ThenInclude(e => e.Empresa)
            .Include(a => a.Prestamo)
                .ThenInclude(p => p.TipoPrestamo)
            .Where(a => !a.StatusPago && a.Prestamo.Estado == "A")
            .AsQueryable();

        var directosQuery = _context.PrestamosDirectos
            .Include(d => d.Prestamo)
                .ThenInclude(p => p.Empleado)
                    .ThenInclude(e => e.Empresa)
            .Include(d => d.Prestamo)
                .ThenInclude(p => p.TipoPrestamo)
            .Where(d => !d.StatusPago && d.Prestamo.Estado == "A")
            .AsQueryable();

        if (empresaId.HasValue && empresaId.Value > 0)
        {
            amortizacionesQuery = amortizacionesQuery.Where(a => a.Prestamo.Empleado.EmpresaId == empresaId.Value);
            directosQuery = directosQuery.Where(d => d.Prestamo.Empleado.EmpresaId == empresaId.Value);
        }

        if (tipoPagoId.HasValue && tipoPagoId.Value > 0)
        {
            amortizacionesQuery = amortizacionesQuery.Where(a => a.Prestamo.TipoPagoId == tipoPagoId.Value);
            directosQuery = directosQuery.Where(d => d.Prestamo.TipoPagoId == tipoPagoId.Value);
        }

        if (fechaVencimiento.HasValue)
        {
            var fecha = fechaVencimiento.Value.Date;
            amortizacionesQuery = amortizacionesQuery.Where(a => a.FechaPago.Date <= fecha);
            directosQuery = directosQuery.Where(d => d.FechaPago.Date <= fecha);
        }

        var amortizaciones = await amortizacionesQuery
            .OrderBy(a => a.FechaPago)
            .ThenBy(a => a.PrestamoId)
            .Select(a => new AmortizacionPendienteDto
            {
                AmortizacionId = a.AmortizacionId,
                PrestamoId = a.PrestamoId,
                NumeroPago = a.NumeroPago,
                FechaPago = a.FechaPago,
                ImportePago = a.ImportePago,
                ImportePagoCapital = a.ImportePagoCapital,
                ImportePagoInteres = a.ImportePagoInteres,
                PagoExtraCapital = a.PagoExtraCapital,
                SaldoFinal = a.SaldoFinal,
                StatusPago = a.StatusPago,
                EsPrestamoDirecto = false,
                EmpleadoId = a.Prestamo.EmpleadoId,
                EmpleadoNombre = a.Prestamo.Empleado.Nombres + " " + a.Prestamo.Empleado.ApellidoPaterno + " " + (a.Prestamo.Empleado.ApellidoMaterno ?? ""),
                EmpresaId = a.Prestamo.Empleado.EmpresaId,
                EmpresaNombre = a.Prestamo.Empleado.Empresa.RazonSocial,
                Folio = a.Prestamo.Folio,
                TipoPrestamo = a.Prestamo.TipoPrestamo.Descripcion
            })
            .ToListAsync();

        var directos = await directosQuery
            .OrderBy(d => d.FechaPago)
            .ThenBy(d => d.PrestamoId)
            .Select(d => new AmortizacionPendienteDto
            {
                AmortizacionId = 0,
                PrestamoId = d.PrestamoId,
                NumeroPago = 0,
                FechaPago = d.FechaPago,
                ImportePago = d.TotalCapitalInteres,
                ImportePagoCapital = d.MontoSolicitado,
                ImportePagoInteres = d.MontoIntereses,
                PagoExtraCapital = 0,
                SaldoFinal = 0,
                StatusPago = false,
                EsPrestamoDirecto = true,
                PrestamoDirectoId = d.NumPrestamoDirecto,
                DescripcionPrestamoDirecto = d.Descripcion,
                MontoSolicitadoDirecto = d.MontoSolicitado,
                MontoInteresesDirecto = d.MontoIntereses,
                TotalPagarDirecto = d.TotalCapitalInteres,
                FechaPagoDirecto = d.FechaPago,
                StatusPagoDirecto = d.StatusPago,
                EmpleadoId = d.Prestamo.EmpleadoId,
                EmpleadoNombre = d.Prestamo.Empleado.Nombres + " " + d.Prestamo.Empleado.ApellidoPaterno + " " + (d.Prestamo.Empleado.ApellidoMaterno ?? ""),
                EmpresaId = d.Prestamo.Empleado.EmpresaId,
                EmpresaNombre = d.Prestamo.Empleado.Empresa.RazonSocial,
                Folio = d.Prestamo.Folio,
                TipoPrestamo = d.Prestamo.TipoPrestamo.Descripcion
            })
            .ToListAsync();

        return amortizaciones
            .Concat(directos)
            .OrderBy(x => x.FechaPago)
            .ThenBy(x => x.PrestamoId)
            .ThenBy(x => x.EsPrestamoDirecto ? 1 : 0)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<bool> AplicarPagoIndividualAsync(AplicarPagoIndividualCommand command, string usuarioId)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (command.EsPrestamoDirecto)
            {
                await ProcesarPagoPrestamoDirectoAsync(
                    command.PrestamoId,
                    command.PrestamoDirectoId,
                    command.MontoRecibido,
                    command.MetodoPago,
                    command.Referencia,
                    usuarioId,
                    command.FechaPago);
            }
            else
            {
                await ProcesarPagoAmortizacionAsync(
                    command.PrestamoId,
                    command.NumeroPago,
                    command.AmortizacionId,
                    command.MontoRecibido,
                    command.MetodoPago,
                    command.Referencia,
                    usuarioId,
                    null,
                    command.FechaPago);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<BulkPaymentResultDto> AplicarPagoMasivoAsync(AplicarPagoMasivoCommand command, string usuarioId)
    {
        ArgumentNullException.ThrowIfNull(command);

        var result = new BulkPaymentResultDto
        {
            TotalRegistros = command.Detalles.Count
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var detalle in command.Detalles)
            {
                try
                {
                    if (detalle.EsPrestamoDirecto)
                    {
                        await ProcesarPagoPrestamoDirectoAsync(
                            detalle.PrestamoId,
                            detalle.PrestamoDirectoId,
                            detalle.MontoAplicado,
                            command.MetodoPago,
                            $"Lote Directo: {command.OrigenNomina}",
                            usuarioId,
                            command.FechaCobro,
                            command.OrigenNomina);
                    }
                    else
                    {
                        await ProcesarPagoAmortizacionAsync(
                            detalle.PrestamoId,
                            detalle.NumeroPago,
                            detalle.AmortizacionId,
                            detalle.MontoAplicado,
                            command.MetodoPago,
                            $"Lote Nómina: {command.OrigenNomina}",
                            usuarioId,
                            command.OrigenNomina,
                            command.FechaCobro);
                    }

                    result.Exitosos++;
                }
                catch (InvalidOperationException ex)
                {
                    result.Fallidos++;
                    result.DetallesErrores.Add($"{ObtenerDescripcionDetalle(detalle)}: {ex.Message}");
                }
            }

            if (result.Fallidos > 0)
            {
                await transaction.RollbackAsync();
                return result;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.Fallidos = command.Detalles.Count;
            result.Exitosos = 0;
            result.DetallesErrores.Add($"Error general de base de datos: {ex.Message}");
            return result;
        }
    }

    private async Task ProcesarPagoAmortizacionAsync(
        int prestamoId,
        int? numeroPago,
        int? amortizacionId,
        decimal montoRecibido,
        string metodoPago,
        string? referencia,
        string usuarioId,
        string? origenNomina = null,
        DateTime? fechaPago = null)
    {
        if (prestamoId <= 0)
            throw new InvalidOperationException("El préstamo es inválido.");

        if (!numeroPago.HasValue || numeroPago.Value <= 0)
            throw new InvalidOperationException("El número de pago es inválido.");

        if (montoRecibido <= 0)
            throw new InvalidOperationException("El monto recibido debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(metodoPago))
            throw new InvalidOperationException("El método de pago es obligatorio.");

        if (string.IsNullOrWhiteSpace(usuarioId))
            throw new InvalidOperationException("El usuario que aplica el cobro es obligatorio.");

        var prestamo = await _context.Prestamos
            .Include(p => p.PrestamosAmortizaciones)
            .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId)
            ?? throw new InvalidOperationException("El préstamo especificado no existe.");

        PrestamosAmortizacione? amortizacion = null;
        if (amortizacionId.HasValue && amortizacionId.Value > 0)
        {
            amortizacion = prestamo.PrestamosAmortizaciones.FirstOrDefault(a => a.AmortizacionId == amortizacionId.Value);
        }

        amortizacion ??= prestamo.PrestamosAmortizaciones.FirstOrDefault(a => a.NumeroPago == numeroPago.Value);

        if (amortizacion is null)
            throw new InvalidOperationException($"No se encontró la cuota número {numeroPago.Value} para este préstamo.");

        if (amortizacion.StatusPago)
            throw new InvalidOperationException($"La cuota número {amortizacion.NumeroPago} ya se encuentra pagada.");

        var cuotasPendientesPrevias = prestamo.PrestamosAmortizaciones
            .Where(a => !a.StatusPago && a.NumeroPago < amortizacion.NumeroPago)
            .ToList();

        if (cuotasPendientesPrevias.Any())
        {
            throw new InvalidOperationException($"No se puede pagar la cuota {amortizacion.NumeroPago} porque la cuota {cuotasPendientesPrevias.First().NumeroPago} aún está pendiente de pago.");
        }

        var cobro = new PrestamoCobro
        {
            PrestamoId = prestamoId,
            FechaCobro = fechaPago ?? DateTime.Now,
            MontoTotal = montoRecibido,
            MetodoPago = metodoPago,
            Referencia = referencia,
            UsuarioId = usuarioId,
            OrigenNomina = origenNomina
        };
        _context.PrestamoCobros.Add(cobro);

        decimal remanente = montoRecibido;
        decimal capitalAplicado = 0;
        decimal interesAplicado = 0;
        decimal extraCapitalAplicado = 0;

        decimal interesPendiente = amortizacion.ImportePagoInteres;
        if (remanente >= interesPendiente)
        {
            interesAplicado = interesPendiente;
            remanente -= interesPendiente;
        }
        else
        {
            interesAplicado = remanente;
            remanente = 0;
        }

        if (remanente > 0)
        {
            decimal capitalPendiente = amortizacion.ImportePagoCapital;
            if (remanente >= capitalPendiente)
            {
                capitalAplicado = capitalPendiente;
                remanente -= capitalPendiente;
            }
            else
            {
                capitalAplicado = remanente;
                remanente = 0;
            }
        }

        amortizacion.ImportePagoInteres -= interesAplicado;
        amortizacion.ImportePagoCapital -= capitalAplicado;
        amortizacion.ImportePago = amortizacion.ImportePagoInteres + amortizacion.ImportePagoCapital;

        if (amortizacion.ImportePagoInteres == 0 && amortizacion.ImportePagoCapital == 0)
        {
            amortizacion.StatusPago = true;
        }

        if (remanente > 0)
        {
            extraCapitalAplicado = remanente;
            amortizacion.PagoExtraCapital += extraCapitalAplicado;
            remanente = 0;
        }

        var detallePago = new PrestamoDetallePago
        {
            Cobro = cobro,
            PrestamoId = prestamoId,
            AmortizacionId = amortizacion.AmortizacionId,
            PrestamoDirectoId = null,
            MontoCapital = capitalAplicado,
            MontoInteres = interesAplicado,
            PagoExtraCapital = extraCapitalAplicado,
            MontoTotal = capitalAplicado + interesAplicado + extraCapitalAplicado
        };
        _context.PrestamoDetallePagos.Add(detallePago);

        var reduccionTotalCapital = capitalAplicado + extraCapitalAplicado;
        prestamo.SaldoPendiente = Math.Max(0, prestamo.SaldoPendiente - reduccionTotalCapital);

        if (extraCapitalAplicado > 0)
        {
            amortizacion.SaldoFinal = Math.Max(0, amortizacion.SaldoFinal - extraCapitalAplicado);

            var cuotasSiguientes = prestamo.PrestamosAmortizaciones
                .Where(a => a.NumeroPago > amortizacion.NumeroPago)
                .OrderBy(a => a.NumeroPago)
                .ToList();

            foreach (var cuotaSig in cuotasSiguientes)
            {
                cuotaSig.SaldoInicial = Math.Max(0, cuotaSig.SaldoInicial - extraCapitalAplicado);
                cuotaSig.SaldoFinal = Math.Max(0, cuotaSig.SaldoFinal - extraCapitalAplicado);
            }
        }
    }

    private async Task ProcesarPagoPrestamoDirectoAsync(
        int prestamoId,
        int? prestamoDirectoId,
        decimal montoRecibido,
        string metodoPago,
        string? referencia,
        string usuarioId,
        DateTime? fechaPago = null,
        string? origenNomina = null)
    {
        if (prestamoId <= 0)
            throw new InvalidOperationException("El préstamo es inválido.");

        if (!prestamoDirectoId.HasValue || prestamoDirectoId.Value <= 0)
            throw new InvalidOperationException("El préstamo directo es inválido.");

        if (montoRecibido <= 0)
            throw new InvalidOperationException("El monto recibido debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(metodoPago))
            throw new InvalidOperationException("El método de pago es obligatorio.");

        if (string.IsNullOrWhiteSpace(usuarioId))
            throw new InvalidOperationException("El usuario que aplica el cobro es obligatorio.");

        var prestamoDirecto = await _context.PrestamosDirectos
            .Include(d => d.Prestamo)
                .ThenInclude(p => p.Empleado)
            .FirstOrDefaultAsync(d => d.PrestamoId == prestamoId && d.NumPrestamoDirecto == prestamoDirectoId.Value)
            ?? throw new InvalidOperationException("El préstamo directo especificado no existe.");

        if (prestamoDirecto.StatusPago)
            throw new InvalidOperationException("El préstamo directo ya se encuentra pagado.");

        var montoEsperado = Math.Round(prestamoDirecto.TotalCapitalInteres, 2);
        if (Math.Round(montoRecibido, 2) != montoEsperado)
            throw new InvalidOperationException($"El monto recibido debe ser exactamente {montoEsperado:C2} para liquidar el préstamo directo.");

        var cobro = new PrestamoCobro
        {
            PrestamoId = prestamoId,
            FechaCobro = fechaPago ?? DateTime.Now,
            MontoTotal = montoRecibido,
            MetodoPago = metodoPago,
            Referencia = referencia,
            UsuarioId = usuarioId,
            OrigenNomina = origenNomina
        };
        _context.PrestamoCobros.Add(cobro);

        var detallePago = new PrestamoDetallePago
        {
            Cobro = cobro,
            PrestamoId = prestamoId,
            AmortizacionId = null,
            PrestamoDirectoId = prestamoDirecto.NumPrestamoDirecto,
            MontoCapital = prestamoDirecto.MontoSolicitado,
            MontoInteres = prestamoDirecto.MontoIntereses,
            PagoExtraCapital = 0,
            MontoTotal = montoRecibido
        };
        _context.PrestamoDetallePagos.Add(detallePago);

        prestamoDirecto.StatusPago = true;
        prestamoDirecto.FechaPagoReal = fechaPago ?? DateTime.Now;
    }

    private static string ObtenerDescripcionDetalle(DetallePagoMasivoDto detalle)
    {
        if (detalle.EsPrestamoDirecto)
        {
            return $"Prestamo ID {detalle.PrestamoId}, Directo #{detalle.PrestamoDirectoId?.ToString() ?? "N/D"}";
        }

        return $"Prestamo ID {detalle.PrestamoId}, Pago #{detalle.NumeroPago?.ToString() ?? "N/D"}";
    }
}

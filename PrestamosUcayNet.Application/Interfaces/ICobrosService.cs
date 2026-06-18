using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.Cobros.Commands;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface ICobrosService
{
    Task<bool> AplicarPagoIndividualAsync(AplicarPagoIndividualCommand command, string usuarioId);
    Task<BulkPaymentResultDto> AplicarPagoMasivoAsync(AplicarPagoMasivoCommand command, string usuarioId);
    Task<List<AmortizacionPendienteDto>> GetAmortizacionesPendientesAsync(int? empresaId, int? tipoPagoId, DateTime? fechaVencimiento);
}

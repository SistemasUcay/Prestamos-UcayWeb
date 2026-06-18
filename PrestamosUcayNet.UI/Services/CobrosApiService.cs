using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.Cobros.Commands;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class CobrosApiService
{
    private readonly HttpClient _http;

    public CobrosApiService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Obtiene las amortizaciones pendientes por cobrar filtradas opcionalmente
    /// </summary>
    public async Task<List<AmortizacionPendienteDto>> GetPendientesAsync(
        int? empresaId = null, 
        int? tipoPagoId = null, 
        DateTime? fechaVencimiento = null)
    {
        var url = "api/cobros/pendientes";
        var queryParams = new List<string>();

        if (empresaId.HasValue && empresaId.Value > 0)
            queryParams.Add($"empresaId={empresaId}");
        
        if (tipoPagoId.HasValue && tipoPagoId.Value > 0)
            queryParams.Add($"tipoPagoId={tipoPagoId}");
        
        if (fechaVencimiento.HasValue)
            queryParams.Add($"fechaVencimiento={fechaVencimiento.Value:yyyy-MM-dd}");

        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        return await _http.GetFromJsonAsync<List<AmortizacionPendienteDto>>(url) ?? [];
    }

    /// <summary>
    /// Aplica un pago individual
    /// </summary>
    public async Task<HttpResponseMessage> AplicarPagoIndividualAsync(AplicarPagoIndividualCommand command)
    {
        return await _http.PostAsJsonAsync("api/cobros/aplicar-individual", command);
    }

    /// <summary>
    /// Aplica un pago masivo
    /// </summary>
    public async Task<HttpResponseMessage> AplicarPagoMasivoAsync(AplicarPagoMasivoCommand command)
    {
        return await _http.PostAsJsonAsync("api/cobros/aplicar-masivo", command);
    }
}

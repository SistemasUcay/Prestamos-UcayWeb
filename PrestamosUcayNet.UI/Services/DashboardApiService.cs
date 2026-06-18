using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class DashboardApiService
{
    private readonly HttpClient _http;

    public DashboardApiService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Consume el endpoint api/dashboard para obtener los datos del panel principal.
    /// </summary>
    public async Task<DashboardDto?> GetDashboardDataAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var url = "api/dashboard";
        var query = new System.Collections.Generic.List<string>();

        if (fechaInicio.HasValue)
            query.Add($"fechaInicio={fechaInicio.Value:yyyy-MM-dd}");

        if (fechaFin.HasValue)
            query.Add($"fechaFin={fechaFin.Value:yyyy-MM-dd}");

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        return await _http.GetFromJsonAsync<DashboardDto>(url);
    }
}

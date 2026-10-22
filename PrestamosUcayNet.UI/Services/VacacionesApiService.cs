using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

/// <summary>
/// Cliente HTTP tipado para interactuar con los endpoints del módulo de Vacaciones desde la UI Blazor.
/// </summary>
public class VacacionesApiService
{
    private readonly HttpClient _http;

    public VacacionesApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ResumenVacacionesEmpleadoDto?> GetResumenPorEmpleadoAsync(int empleadoId)
    {
        return await _http.GetFromJsonAsync<ResumenVacacionesEmpleadoDto>($"api/vacaciones/empleado/{empleadoId}/resumen");
    }

    public async Task<List<PeriodoVacacionalDto>> GetPeriodosPorEmpleadoAsync(int empleadoId)
    {
        return await _http.GetFromJsonAsync<List<PeriodoVacacionalDto>>($"api/vacaciones/empleado/{empleadoId}/periodos") ?? new();
    }

    public async Task<List<SolicitudVacacionesListDto>> GetSolicitudesAsync(
        int? empleadoId = null,
        int? empresaId = null,
        string? estado = null)
    {
        var queryParams = new List<string>();
        if (empleadoId.HasValue) queryParams.Add($"empleadoId={empleadoId.Value}");
        if (empresaId.HasValue) queryParams.Add($"empresaId={empresaId.Value}");
        if (!string.IsNullOrWhiteSpace(estado)) queryParams.Add($"estado={Uri.EscapeDataString(estado)}");

        var url = "api/vacaciones/solicitudes";
        if (queryParams.Count > 0)
            url += "?" + string.Join("&", queryParams);

        return await _http.GetFromJsonAsync<List<SolicitudVacacionesListDto>>(url) ?? new();
    }

    public async Task<SolicitudVacacionesDetailDto?> GetSolicitudByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<SolicitudVacacionesDetailDto>($"api/vacaciones/solicitudes/{id}");
    }

    public async Task<HttpResponseMessage> CreateSolicitudAsync(CrearSolicitudVacacionesDto dto)
    {
        return await _http.PostAsJsonAsync("api/vacaciones/solicitudes", dto);
    }

    public async Task<HttpResponseMessage> ResolverSolicitudAsync(int id, AprobarRechazarSolicitudDto dto)
    {
        return await _http.PutAsJsonAsync($"api/vacaciones/solicitudes/{id}/resolver", dto);
    }

    public async Task<HttpResponseMessage> CancelarSolicitudAsync(int id)
    {
        return await _http.PutAsync($"api/vacaciones/solicitudes/{id}/cancelar", null);
    }

    public async Task<HttpResponseMessage> GenerarPeriodosEmpleadoAsync(int empleadoId)
    {
        return await _http.PostAsync($"api/vacaciones/empleado/{empleadoId}/generar-periodos", null);
    }

    public async Task<HttpResponseMessage> AplicarCaducidadAsync(int? empleadoId = null)
    {
        var url = "api/vacaciones/aplicar-caducidad";
        if (empleadoId.HasValue)
            url += $"?empleadoId={empleadoId.Value}";

        return await _http.PostAsync(url, null);
    }
}

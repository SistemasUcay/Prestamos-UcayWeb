using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class SolicitudesApiService
{
    private readonly HttpClient _http;

    public SolicitudesApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CapacidadPagoResultDto> EvaluarCapacidadAsync(CapacidadPagoDto request)
    {
        var response = await _http.PostAsJsonAsync("api/solicitudes/evaluar-capacidad", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CapacidadPagoResultDto>() ?? new();
    }

    public async Task<bool> CrearSolicitudAsync(CrearSolicitudDto request)
    {
        var response = await _http.PostAsJsonAsync("api/solicitudes", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<SolicitudDetalleDto>> GetTodasAsync()
    {
        return await _http.GetFromJsonAsync<List<SolicitudDetalleDto>>("api/solicitudes") ?? new();
    }

    public async Task<List<SolicitudDetalleDto>> GetPendientesAsync()
    {
        return await _http.GetFromJsonAsync<List<SolicitudDetalleDto>>("api/solicitudes/pendientes") ?? new();
    }

    public async Task<List<SolicitudDetalleDto>> GetAprobadasAsync()
    {
        // En el backend no creamos endpoint específico aún, pero podemos filtrar o crear uno.
        // Por ahora, asumamos que necesitamos un endpoint o filtramos en el cliente (no recomendado).
        // Vamos a crear el endpoint en el backend después.
        return await _http.GetFromJsonAsync<List<SolicitudDetalleDto>>("api/solicitudes/aprobadas") ?? new();
    }

    public async Task<SolicitudDetalleDto?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<SolicitudDetalleDto>($"api/solicitudes/{id}");
    }

    public async Task<bool> EditarAsync(EditarSolicitudDto request)
    {
        var response = await _http.PutAsJsonAsync("api/solicitudes", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AprobarAsync(AprobarSolicitudDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/solicitudes/aprobar", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RechazarAsync(RechazarSolicitudDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/solicitudes/rechazar", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CancelarAsync(CancelarSolicitudDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/solicitudes/cancelar", dto);
        return response.IsSuccessStatusCode;
    }
}

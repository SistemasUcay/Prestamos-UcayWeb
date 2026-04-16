using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class EmpleadosApiService
{
    private readonly HttpClient _http;

    public EmpleadosApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EmpleadoListDto>> GetEmpleadosAsync(int? empresaId = null)
    {
        var url = "api/empleados";
        if (empresaId.HasValue) url += $"?empresaId={empresaId}";
        
        return await _http.GetFromJsonAsync<List<EmpleadoListDto>>(url) ?? new();
    }

    public async Task<EmpleadoDetailDto?> GetEmpleadoByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<EmpleadoDetailDto>($"api/empleados/{id}");
    }

    public async Task<HttpResponseMessage> CreateEmpleadoAsync(EmpleadoCreateUpdateDto dto)
    {
        return await _http.PostAsJsonAsync("api/empleados", dto);
    }

    public async Task<HttpResponseMessage> UpdateEmpleadoAsync(int id, EmpleadoCreateUpdateDto dto)
    {
        return await _http.PutAsJsonAsync($"api/empleados/{id}", dto);
    }

    public async Task<HttpResponseMessage> DeleteEmpleadoAsync(int id)
    {
        return await _http.DeleteAsync($"api/empleados/{id}");
    }
}

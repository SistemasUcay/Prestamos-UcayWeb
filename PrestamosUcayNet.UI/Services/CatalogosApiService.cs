using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class CatalogosApiService
{
    private readonly HttpClient _http;

    public CatalogosApiService(HttpClient http)
    {
        _http = http;
    }

    // --- Departamentos ---

    public async Task<List<DepartamentoDto>> GetDepartamentosAsync()
    {
        return await _http.GetFromJsonAsync<List<DepartamentoDto>>("api/catalogos/departamentos") ?? new();
    }

    public async Task<DepartamentoDto?> GetDepartamentoByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<DepartamentoDto>($"api/catalogos/departamentos/{id}");
    }

    public async Task<bool> CreateDepartamentoAsync(DepartamentoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/catalogos/departamentos", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateDepartamentoAsync(DepartamentoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/catalogos/departamentos/{dto.DepartamentoId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteDepartamentoAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/catalogos/departamentos/{id}");
        return response.IsSuccessStatusCode;
    }

    // --- Puestos ---

    public async Task<List<PuestoDto>> GetPuestosAsync()
    {
        return await _http.GetFromJsonAsync<List<PuestoDto>>("api/catalogos/puestos") ?? new();
    }

    public async Task<PuestoDto?> GetPuestoByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<PuestoDto>($"api/catalogos/puestos/{id}");
    }

    public async Task<bool> CreatePuestoAsync(PuestoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/catalogos/puestos", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePuestoAsync(PuestoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/catalogos/puestos/{dto.PuestoId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePuestoAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/catalogos/puestos/{id}");
        return response.IsSuccessStatusCode;
    }

    // --- Tipos de Pago (Frecuencia de Pago) ---

    public async Task<List<TipoPagoDto>> GetTiposPagoAsync()
    {
        return await _http.GetFromJsonAsync<List<TipoPagoDto>>("api/catalogos/tipospago") ?? new();
    }

    public async Task<TipoPagoDto?> GetTipoPagoByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<TipoPagoDto>($"api/catalogos/tipospago/{id}");
    }

    public async Task<bool> CreateTipoPagoAsync(TipoPagoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/catalogos/tipospago", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTipoPagoAsync(TipoPagoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/catalogos/tipospago/{dto.TipoPagoId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTipoPagoAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/catalogos/tipospago/{id}");
        return response.IsSuccessStatusCode;
    }

    // --- Tipos de Préstamo ---

    public async Task<List<TipoPrestamoDto>> GetTiposPrestamoAsync()
    {
        return await _http.GetFromJsonAsync<List<TipoPrestamoDto>>("api/catalogos/tipos-prestamo") ?? new();
    }

    public async Task<TipoPrestamoDto?> GetTipoPrestamoByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<TipoPrestamoDto>($"api/catalogos/tipos-prestamo/{id}");
    }

    public async Task<bool> CreateTipoPrestamoAsync(TipoPrestamoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/catalogos/tipos-prestamo", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTipoPrestamoAsync(TipoPrestamoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/catalogos/tipos-prestamo/{dto.TipoPrestamoId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTipoPrestamoAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/catalogos/tipos-prestamo/{id}");
        return response.IsSuccessStatusCode;
    }

    // --- Condiciones de Crédito ---

    public async Task<List<CondicionCreditoDto>> GetCondicionesCreditoAsync()
    {
        return await _http.GetFromJsonAsync<List<CondicionCreditoDto>>("api/catalogos/condiciones-credito") ?? new();
    }

    public async Task<List<CondicionCreditoDto>> GetCondicionesCreditoPorFondeadoraAsync(int fondeadoraId)
    {
        return await _http.GetFromJsonAsync<List<CondicionCreditoDto>>($"api/catalogos/condiciones-credito/fondeadora/{fondeadoraId}") ?? new();
    }
}

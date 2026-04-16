using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class EmpresasApiService
{
    private readonly HttpClient _http;

    public EmpresasApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EmpresaListDto>> GetEmpresasAsync()
    {
        return await _http.GetFromJsonAsync<List<EmpresaListDto>>("api/empresas") ?? new();
    }

    public async Task<EmpresaDetailDto?> GetEmpresaByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<EmpresaDetailDto>($"api/empresas/{id}");
    }

    public async Task<HttpResponseMessage> CreateEmpresaAsync(EmpresaCreateUpdateDto dto)
    {
        return await _http.PostAsJsonAsync("api/empresas", dto);
    }

    public async Task<HttpResponseMessage> UpdateEmpresaAsync(int id, EmpresaCreateUpdateDto dto)
    {
        return await _http.PutAsJsonAsync($"api/empresas/{id}", dto);
    }

    public async Task<HttpResponseMessage> DeleteEmpresaAsync(int id)
    {
        return await _http.DeleteAsync($"api/empresas/{id}");
    }
}

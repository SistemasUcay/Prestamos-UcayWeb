using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class ParametrosApiService
{
    private readonly HttpClient _http;

    public ParametrosApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ParametrosDto> GetParametrosAsync()
    {
        return await _http.GetFromJsonAsync<ParametrosDto>("api/parametros") ?? new();
    }

    public async Task<HttpResponseMessage> SaveParametrosAsync(ParametrosDto dto)
    {
        return await _http.PostAsJsonAsync("api/parametros", dto);
    }
}

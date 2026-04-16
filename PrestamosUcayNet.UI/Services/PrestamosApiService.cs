using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.UI.Services;

public class PrestamosApiService
{
    private readonly HttpClient _http;

    public PrestamosApiService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Obtiene todos los préstamos
    /// </summary>
    public async Task<List<PrestamoListDto>> GetTodosAsync()
    {
        return await _http.GetFromJsonAsync<List<PrestamoListDto>>("api/prestamos") ?? [];
    }

    /// <summary>
    /// Obtiene préstamos filtrados por estado
    /// </summary>
    public async Task<List<PrestamoListDto>> GetPorEstadoAsync(string estado)
    {
        return await _http.GetFromJsonAsync<List<PrestamoListDto>>($"api/prestamos/por-estado/{estado}") ?? [];
    }

    /// <summary>
    /// Obtiene préstamos de un empleado
    /// </summary>
    public async Task<List<PrestamoListDto>> GetPorEmpleadoAsync(int empleadoId)
    {
        return await _http.GetFromJsonAsync<List<PrestamoListDto>>($"api/prestamos/por-empleado/{empleadoId}") ?? [];
    }

    /// <summary>
    /// Obtiene el detalle completo de un préstamo
    /// </summary>
    public async Task<PrestamoDetalleDto?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<PrestamoDetalleDto>($"api/prestamos/{id}");
    }

    /// <summary>
    /// Crea un préstamo directamente
    /// </summary>
    public async Task<bool> CrearAsync(PrestamoCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/prestamos", dto);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Simula la tabla de amortización sin guardar, para previsualización
    /// </summary>
    public async Task<AmortizacionPreviewResultDto?> SimularAmortizacionAsync(SimularAmortizacionDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/prestamos/simular-amortizacion", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AmortizacionPreviewResultDto>();
    }

    /// <summary>
    /// Activa un préstamo desde solicitud aprobada
    /// </summary>
    public async Task<bool> ActivarAsync(ActivarPrestamoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/prestamos/activar", dto);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Actualiza campos editables de un préstamo
    /// </summary>
    public async Task<bool> ActualizarAsync(PrestamoUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync("api/prestamos", dto);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Cancela un préstamo activo
    /// </summary>
    public async Task<bool> CancelarAsync(int prestamoId)
    {
        var response = await _http.PostAsync($"api/prestamos/{prestamoId}/cancelar", null);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Registra un pago al préstamo
    /// </summary>
    public async Task<bool> RegistrarPagoAsync(RegistrarPagoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/prestamos/registrar-pago", dto);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Solicita pausa de cuotas del préstamo
    /// </summary>
    public async Task<bool> PausarAsync(SolicitudPausaDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/prestamos/pausar", dto);
        return response.IsSuccessStatusCode;
    }
}

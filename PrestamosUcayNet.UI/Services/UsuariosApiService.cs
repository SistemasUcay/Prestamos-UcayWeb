using System.Net.Http.Json;
using PrestamosUcayNet.UI.Models;

namespace PrestamosUcayNet.UI.Services;

/// <summary>
/// Servicio para gestionar usuarios desde la UI (solo Administrador).
/// </summary>
public class UsuariosApiService
{
    private readonly HttpClient _http;

    public UsuariosApiService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Obtiene la lista de todos los usuarios.
    /// </summary>
    public async Task<List<UsuarioListModel>> ObtenerTodosAsync()
    {
        var result = await _http.GetFromJsonAsync<List<UsuarioListModel>>("api/usuarios");
        return result ?? [];
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    public async Task<(bool Success, string? Error)> RegistrarAsync(RegistroUsuarioModel model)
    {
        var response = await _http.PostAsJsonAsync("api/usuarios", model);
        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        return (false, error?.Error ?? "Error al crear usuario.");
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    public async Task<(bool Success, string? Error)> ActualizarAsync(string id, ActualizarUsuarioModel model)
    {
        var response = await _http.PutAsJsonAsync($"api/usuarios/{id}", model);
        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        return (false, error?.Error ?? "Error al actualizar usuario.");
    }

    /// <summary>
    /// Desactiva un usuario.
    /// </summary>
    public async Task<(bool Success, string? Error)> DesactivarAsync(string id)
    {
        var response = await _http.DeleteAsync($"api/usuarios/{id}");
        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        return (false, error?.Error ?? "Error al desactivar usuario.");
    }

    /// <summary>
    /// Obtiene los roles disponibles.
    /// </summary>
    public async Task<List<string>> ObtenerRolesAsync()
    {
        var result = await _http.GetFromJsonAsync<List<string>>("api/usuarios/roles");
        return result ?? [];
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    public async Task<(bool Success, string? Error)> CambiarPasswordAsync(string id, CambiarPasswordModel model)
    {
        var response = await _http.PutAsJsonAsync($"api/usuarios/{id}/password", model);
        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        return (false, error?.Error ?? "Error al cambiar contraseña.");
    }
}

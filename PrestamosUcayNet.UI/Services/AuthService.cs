using System.Net.Http.Json;
using PrestamosUcayNet.UI.Models;

namespace PrestamosUcayNet.UI.Services;

/// <summary>
/// Servicio de autenticación que se comunica con el API.
/// </summary>
public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly JwtAuthStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, JwtAuthStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Inicia sesión en el sistema.
    /// </summary>
    public async Task<(bool Success, string? Error)> LoginAsync(LoginModel model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                if (result != null)
                {
                    await _authStateProvider.MarkUserAsAuthenticated(result.Token);
                    return (true, null);
                }
                return (false, "Respuesta inesperada del servidor.");
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return (false, error?.Error ?? "Error de autenticación.");
            }
        }
        catch (Exception ex)
        {
            return (false, $"Error de conexión: {ex.Message}");
        }
    }

    /// <summary>
    /// Cierra la sesión del usuario.
    /// </summary>
    public async Task LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOut();
    }
}

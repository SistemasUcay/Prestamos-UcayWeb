using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

/// <summary>
/// Servicio de gestión de usuarios y autenticación del sistema.
/// </summary>
public interface IUsuariosService
{
    /// <summary>
    /// Autentica un usuario y genera un token JWT.
    /// </summary>
    Task<LoginResponseDto> LoginAsync(LoginDto dto);

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    Task<string> RegistrarUsuarioAsync(RegistroUsuarioDto dto);

    /// <summary>
    /// Obtiene la lista de todos los usuarios del sistema.
    /// </summary>
    Task<List<UsuarioListDto>> ObtenerTodosAsync();

    /// <summary>
    /// Obtiene un usuario por su Id.
    /// </summary>
    Task<UsuarioListDto?> ObtenerPorIdAsync(string id);

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    Task ActualizarUsuarioAsync(string id, ActualizarUsuarioDto dto);

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    Task CambiarPasswordAsync(string id, CambiarPasswordDto dto);

    /// <summary>
    /// Desactiva un usuario del sistema (soft delete).
    /// </summary>
    Task DesactivarUsuarioAsync(string id);

    /// <summary>
    /// Obtiene la lista de roles disponibles en el sistema.
    /// </summary>
    Task<List<string>> ObtenerRolesAsync();
}

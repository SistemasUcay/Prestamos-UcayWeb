namespace PrestamosUcayNet.UI.Models;

/// <summary>
/// DTO para la solicitud de inicio de sesión.
/// </summary>
public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO con la respuesta del inicio de sesión exitoso.
/// </summary>
public class LoginResponseModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}

/// <summary>
/// DTO para registrar un nuevo usuario.
/// </summary>
public class RegistroUsuarioModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? EmpleadoId { get; set; }
}

/// <summary>
/// DTO para listar usuarios.
/// </summary>
public class UsuarioListModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public int? EmpleadoId { get; set; }
}

/// <summary>
/// DTO para actualizar un usuario.
/// </summary>
public class ActualizarUsuarioModel
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public List<string> Roles { get; set; } = [];
    public int? EmpleadoId { get; set; }
}

/// <summary>
/// DTO para cambio de contraseña.
/// </summary>
public class CambiarPasswordModel
{
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNuevo { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta genérica de error de la API.
/// </summary>
public class ApiErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Detalle { get; set; }
}

namespace PrestamosUcayNet.Application.DTOs;

/// <summary>
/// DTO para la solicitud de inicio de sesión.
/// </summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO con la respuesta del inicio de sesión exitoso.
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}

/// <summary>
/// DTO para registrar un nuevo usuario en el sistema.
/// </summary>
public class RegistroUsuarioDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? EmpleadoId { get; set; }
}

/// <summary>
/// DTO para listar usuarios del sistema.
/// </summary>
public class UsuarioListDto
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
/// DTO para cambiar la contraseña de un usuario.
/// </summary>
public class CambiarPasswordDto
{
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNuevo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para actualizar datos de un usuario existente.
/// </summary>
public class ActualizarUsuarioDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public List<string> Roles { get; set; } = [];
    public int? EmpleadoId { get; set; }
}

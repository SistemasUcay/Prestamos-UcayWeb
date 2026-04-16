namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Clase base para el usuario del sistema.
/// La implementación concreta con IdentityUser se encuentra en Infrastructure.
/// Aquí se definen solo las propiedades de negocio.
/// </summary>
public class UsuarioSistemaBase
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el usuario está activo en el sistema.
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Fecha de creación de la cuenta.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora del último acceso al sistema.
    /// </summary>
    public DateTime? UltimoAcceso { get; set; }

    /// <summary>
    /// FK opcional para vincular con un empleado del sistema.
    /// </summary>
    public int? EmpleadoId { get; set; }
}

using Microsoft.AspNetCore.Identity;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Infrastructure.Identity;

/// <summary>
/// Usuario del sistema que extiende IdentityUser con propiedades de negocio.
/// </summary>
public class UsuarioSistema : IdentityUser
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

    /// <summary>
    /// Navegación al empleado vinculado.
    /// </summary>
    public virtual Empleado? Empleado { get; set; }
}

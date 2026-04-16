using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.API.Controllers;

/// <summary>
/// Controlador para la gestión de usuarios del sistema.
/// Solo accesible por el rol Administrador (excepto cambio de contraseña propia).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RolSistema.Administrador)]
public class UsuariosController : ControllerBase
{
    private readonly IUsuariosService _usuariosService;

    public UsuariosController(IUsuariosService usuariosService)
    {
        _usuariosService = usuariosService;
    }

    /// <summary>
    /// Obtiene la lista de todos los usuarios del sistema.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        try
        {
            var usuarios = await _usuariosService.ObtenerTodosAsync();
            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al obtener usuarios.", Detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un usuario por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(string id)
    {
        try
        {
            var usuario = await _usuariosService.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { Error = "Usuario no encontrado." });
            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al obtener el usuario.", Detalle = ex.Message });
        }
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDto dto)
    {
        try
        {
            var userId = await _usuariosService.RegistrarUsuarioAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = userId },
                new { Id = userId, Mensaje = "Usuario creado exitosamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al crear el usuario.", Detalle = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(string id, [FromBody] ActualizarUsuarioDto dto)
    {
        try
        {
            await _usuariosService.ActualizarUsuarioAsync(id, dto);
            return Ok(new { Mensaje = "Usuario actualizado exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al actualizar el usuario.", Detalle = ex.Message });
        }
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// Accesible por Admin o por el propio usuario.
    /// </summary>
    [HttpPut("{id}/password")]
    [Authorize] // Cualquier usuario autenticado puede cambiar su propia contraseña
    public async Task<IActionResult> CambiarPassword(string id, [FromBody] CambiarPasswordDto dto)
    {
        try
        {
            // Verificar que sea el propio usuario o un administrador
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole(RolSistema.Administrador);

            if (currentUserId != id && !isAdmin)
                return Forbid();

            await _usuariosService.CambiarPasswordAsync(id, dto);
            return Ok(new { Mensaje = "Contraseña actualizada exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al cambiar la contraseña.", Detalle = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un usuario del sistema (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(string id)
    {
        try
        {
            await _usuariosService.DesactivarUsuarioAsync(id);
            return Ok(new { Mensaje = "Usuario desactivado exitosamente." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al desactivar el usuario.", Detalle = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene la lista de roles disponibles en el sistema.
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> ObtenerRoles()
    {
        try
        {
            var roles = await _usuariosService.ObtenerRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error al obtener roles.", Detalle = ex.Message });
        }
    }
}

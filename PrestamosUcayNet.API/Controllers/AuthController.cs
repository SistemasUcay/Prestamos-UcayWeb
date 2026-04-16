using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;

namespace PrestamosUcayNet.API.Controllers;

/// <summary>
/// Controlador de autenticación del sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuariosService _usuariosService;

    public AuthController(IUsuariosService usuariosService)
    {
        _usuariosService = usuariosService;
    }

    /// <summary>
    /// Inicia sesión y devuelve un token JWT.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _usuariosService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error interno del servidor.", Detalle = ex.Message });
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Infrastructure.Data;
using PrestamosUcayNet.Infrastructure.Identity;

namespace PrestamosUcayNet.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de usuarios usando ASP.NET Core Identity y JWT.
/// </summary>
public class UsuariosService : IUsuariosService
{
    private readonly UserManager<UsuarioSistema> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly UcayNetDbContext _context;

    public UsuariosService(
        UserManager<UsuarioSistema> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        UcayNetDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _context = context;
    }

    /// <inheritdoc />
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !user.Activo)
            throw new UnauthorizedAccessException("Credenciales inválidas o usuario desactivado.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
            throw new UnauthorizedAccessException("Credenciales inválidas o usuario desactivado.");

        // Si el usuario no tiene EmpleadoId vinculado, intentar vincular por correo electrónico
        if (!user.EmpleadoId.HasValue && !string.IsNullOrWhiteSpace(user.Email))
        {
            var empleadoId = await _context.Empleados
                .AsNoTracking()
                .Where(e => e.Email == user.Email)
                .Select(e => (int?)e.EmpleadoId)
                .FirstOrDefaultAsync();

            if (empleadoId.HasValue)
            {
                user.EmpleadoId = empleadoId;
            }
        }

        // Actualizar último acceso
        user.UltimoAcceso = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerarToken(user, roles);

        return new LoginResponseDto
        {
            Token = token,
            Expiracion = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "480")),
            NombreCompleto = user.NombreCompleto,
            Email = user.Email!,
            Roles = roles.ToList(),
            EmpleadoId = user.EmpleadoId
        };
    }

    /// <inheritdoc />
    public async Task<string> RegistrarUsuarioAsync(RegistroUsuarioDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Ya existe un usuario con este correo electrónico.");

        var empleadoId = dto.EmpleadoId;
        if (!empleadoId.HasValue && !string.IsNullOrWhiteSpace(dto.Email))
        {
            empleadoId = await _context.Empleados
                .AsNoTracking()
                .Where(e => e.Email == dto.Email)
                .Select(e => (int?)e.EmpleadoId)
                .FirstOrDefaultAsync();
        }

        var user = new UsuarioSistema
        {
            UserName = dto.Email,
            Email = dto.Email,
            NombreCompleto = dto.NombreCompleto,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            EmailConfirmed = true,
            EmpleadoId = empleadoId
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Error al crear el usuario: {errors}");
        }

        if (!string.IsNullOrWhiteSpace(dto.Rol))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Rol);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al asignar el rol: {errors}");
            }
        }

        return user.Id;
    }

    /// <inheritdoc />
    public async Task<List<UsuarioListDto>> ObtenerTodosAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UsuarioListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UsuarioListDto
            {
                Id = user.Id,
                Email = user.Email!,
                NombreCompleto = user.NombreCompleto,
                Roles = roles.ToList(),
                Activo = user.Activo,
                FechaCreacion = user.FechaCreacion,
                UltimoAcceso = user.UltimoAcceso,
                EmpleadoId = user.EmpleadoId
            });
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<UsuarioListDto?> ObtenerPorIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UsuarioListDto
        {
            Id = user.Id,
            Email = user.Email!,
            NombreCompleto = user.NombreCompleto,
            Roles = roles.ToList(),
            Activo = user.Activo,
            FechaCreacion = user.FechaCreacion,
            UltimoAcceso = user.UltimoAcceso,
            EmpleadoId = user.EmpleadoId
        };
    }

    /// <inheritdoc />
    public async Task ActualizarUsuarioAsync(string id, ActualizarUsuarioDto dto)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new ArgumentException("Usuario no encontrado.");

        user.NombreCompleto = dto.NombreCompleto;
        user.Email = dto.Email;
        user.UserName = dto.Email;
        user.Activo = dto.Activo;
        user.EmpleadoId = dto.EmpleadoId;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Error al actualizar el usuario: {errors}");
        }

        // Actualizar roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Except(dto.Roles).ToList();
        var rolesToAdd = dto.Roles.Except(currentRoles).ToList();

        if (rolesToRemove.Count > 0)
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        if (rolesToAdd.Count > 0)
            await _userManager.AddToRolesAsync(user, rolesToAdd);
    }

    /// <inheritdoc />
    public async Task CambiarPasswordAsync(string id, CambiarPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new ArgumentException("Usuario no encontrado.");

        var result = await _userManager.ChangePasswordAsync(user, dto.PasswordActual, dto.PasswordNuevo);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Error al cambiar la contraseña: {errors}");
        }
    }

    /// <inheritdoc />
    public async Task DesactivarUsuarioAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new ArgumentException("Usuario no encontrado.");

        user.Activo = false;
        await _userManager.UpdateAsync(user);
    }

    /// <inheritdoc />
    public async Task<List<string>> ObtenerRolesAsync()
    {
        return await _roleManager.Roles
            .Select(r => r.Name!)
            .ToListAsync();
    }

    /// <summary>
    /// Genera un token JWT con los claims del usuario y sus roles.
    /// </summary>
    private string GenerarToken(UsuarioSistema user, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada.")));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.NombreCompleto),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.EmpleadoId.HasValue)
            claims.Add(new Claim("empleadoId", user.EmpleadoId.Value.ToString()));

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "480");
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

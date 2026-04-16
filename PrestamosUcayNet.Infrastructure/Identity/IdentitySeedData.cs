using Microsoft.AspNetCore.Identity;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Infrastructure.Identity;

/// <summary>
/// Siembra los datos iniciales de Identity: roles del sistema y usuario administrador.
/// </summary>
public static class IdentitySeedData
{
    /// <summary>
    /// Crea los roles definidos en el SRS y un usuario administrador por defecto.
    /// </summary>
    public static async Task SeedAsync(
        UserManager<UsuarioSistema> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Crear roles del sistema
        foreach (var rol in RolSistema.TodosLosRoles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        // Crear usuario administrador por defecto
        const string adminEmail = "admin@ucaynet.com";
        const string adminPassword = "Admin@123456";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new UsuarioSistema
            {
                UserName = adminEmail,
                Email = adminEmail,
                NombreCompleto = "Administrador del Sistema",
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RolSistema.Administrador);
            }
        }
    }
}

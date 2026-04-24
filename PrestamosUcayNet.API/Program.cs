using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrestamosUcayNet.Infrastructure.Data;
using PrestamosUcayNet.Infrastructure.Identity;
using PrestamosUcayNet.Infrastructure.Services;
using PrestamosUcayNet.Domain.Interfaces;
using PrestamosUcayNet.Infrastructure.Repositories;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Application.Mappings;
using PrestamosUcayNet.Application.Services;
using InfraServices = PrestamosUcayNet.Infrastructure.Services;

// Formato numérico y de fechas de México
var culturaMx = new CultureInfo("es-MX");
CultureInfo.DefaultThreadCurrentCulture = culturaMx;
CultureInfo.DefaultThreadCurrentUICulture = culturaMx;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=UcayNetDB;Trusted_Connection=True;Encrypt=False;";

builder.Services.AddDbContext<UcayNetDbContext>(options =>
    options.UseSqlServer(connectionString));

// ── ASP.NET Core Identity ──
builder.Services.AddIdentity<UsuarioSistema, IdentityRole>(options =>
{
    // Política de contraseñas fuerte (RNF-09 del SRS)
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;

    // Configuración de bloqueo
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<UcayNetDbContext>()
.AddDefaultTokenProviders();

// ── Autenticación JWT ──
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JwtSettings:Key no está configurada.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── AutoMapper ──
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(EmpleadoProfile).Assembly));

// ── Servicios de aplicación ──
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICalculadoraAmortizacionService, CalculadoraAmortizacionService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IEmpresasService, EmpresasService>();
builder.Services.AddScoped<IEmpleadosService, EmpleadosService>();
builder.Services.AddScoped<ICatalogosService, CatalogosService>();
builder.Services.AddScoped<IParametrosService, ParametrosService>();
builder.Services.AddScoped<IPrestamosService, InfraServices.PrestamosService>();

// ── Controllers y CORS ──
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Seed de datos iniciales (roles + usuario admin) ──
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UsuarioSistema>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeedData.SeedAsync(userManager, roleManager);
}

// ── Pipeline HTTP ──
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorUI");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

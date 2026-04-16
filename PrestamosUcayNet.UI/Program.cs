using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PrestamosUcayNet.UI;
using PrestamosUcayNet.UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// HttpClient base — URL configurada en wwwroot/appsettings.json (evita Mixed Content entre HTTP/HTTPS)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl no está configurada en appsettings.json.");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Autenticación
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddAuthorizationCore();

// Servicios de la aplicación
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SolicitudesApiService>();
builder.Services.AddScoped<PrestamosApiService>();
builder.Services.AddScoped<UsuariosApiService>();
builder.Services.AddScoped<EmpresasApiService>();
builder.Services.AddScoped<EmpleadosApiService>();
builder.Services.AddScoped<CatalogosApiService>();
builder.Services.AddScoped<ParametrosApiService>();

await builder.Build().RunAsync();

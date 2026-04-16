# Clean Architecture & .NET 10 Implementation Playbook

Este playbook contiene patrones detallados de implementación para aplicaciones .NET 10 siguiendo los principios de Arquitectura Limpia (Clean Architecture).

## Estructura de Proyecto Recomendada (Arquitectura Limpia)

1. **Domain**: Contiene las entidades, interfaces (repositorios) y excepciones de dominio. No tiene dependencias externas.
2. **Application**: Contiene la lógica de negocio, casos de uso, DTOs y manejadores de comandos/consultas (CQRS). Depende únicamente de `Domain`.
3. **Infrastructure**: Implementaciones de los repositorios usando EF Core o Dapper, servicios externos, acceso a la base de datos, caché (Redis). Depende de `Application`.
4. **Presentation/API**: Los Controladores, Middlewares, configuración de inyección de dependencias (DI). Depende de `Application` (y de `Infrastructure` solo para registrar dependencias en el contenedor DI).

## Mejores Prácticas en .NET 10

- Utiliza características avanzadas de C# 14 (.NET 10).
- Utiliza **Primary Constructors** para inyección de dependencias más limpia.
- Emplea **Minimal APIs** para endpoints simples y optimizados, o Controladores MVC para lógica compleja bien separada.
- **Inyección de Dependencias**: Registra todo en extensiones de servicios (`IServiceCollection`).
- **Resiliencia**: Utiliza `Polly` para reintentos y Circuit Breaker, aprovechando `Microsoft.Extensions.Resilience`.
- **Acceso a Datos**: Prefiere `EF Core` para las operaciones de acceso a datos.


## Configuración y Opciones

Usa siempre el patrón `IOptions<T>` para inyectar configuración fuertemente tipada.

```csharp
// Definición
public class DatabaseOptions
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = string.Empty;
}

// Registro
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));
```

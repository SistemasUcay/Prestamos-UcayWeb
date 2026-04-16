namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Define los roles del sistema según el SRS (Sección 2.3).
/// </summary>
public static class RolSistema
{
    public const string Administrador = "Administrador";
    public const string GestorPrestamos = "GestorPrestamos";
    public const string Supervisor = "Supervisor";
    public const string Consultor = "Consultor";

    /// <summary>
    /// Devuelve todos los roles definidos en el sistema.
    /// </summary>
    public static readonly string[] TodosLosRoles =
    [
        Administrador,
        GestorPrestamos,
        Supervisor,
        Consultor
    ];
}

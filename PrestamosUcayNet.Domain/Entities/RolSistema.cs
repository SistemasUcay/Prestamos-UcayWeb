namespace PrestamosUcayNet.Domain.Entities;

/// <summary>
/// Define los roles del sistema según el SRS (Sección 2.3).
/// </summary>
public static class RolSistema
{
    public const string Administrador = "Administrador";
    public const string Supervisor = "Supervisor";
    public const string GestorPrestamos = "GestorPrestamos";
    public const string RecursosHumanos = "RecursosHumanos";
    public const string Consultor = "Consultor";
    public const string Empleado = "Empleado";

    /// <summary>
    /// Roles con facultad para autorizar/aprobar o rechazar solicitudes de préstamos.
    /// </summary>
    public const string RolesAprobadoresPrestamos = "Administrador,Supervisor";

    /// <summary>
    /// Roles con acceso operativo a captura, activación y cobranza de préstamos.
    /// </summary>
    public const string RolesOperacionPrestamos = "Administrador,Supervisor,GestorPrestamos";

    /// <summary>
    /// Roles con acceso de consulta a préstamos y cartera.
    /// </summary>
    public const string RolesConsultaPrestamos = "Administrador,Supervisor,GestorPrestamos,Consultor";

    /// <summary>
    /// Roles con facultad para autorizar/aprobar o rechazar solicitudes de vacaciones.
    /// </summary>
    public const string RolesAprobadoresVacaciones = "Administrador,Supervisor,RecursosHumanos";

    /// <summary>
    /// Roles con acceso al módulo de vacaciones (gestión, consulta o autoservicio de empleado).
    /// </summary>
    public const string RolesAccesoVacaciones = "Administrador,Supervisor,RecursosHumanos,Consultor,Empleado";

    /// <summary>
    /// Roles con acceso a la administración de catálogos generales.
    /// </summary>
    public const string RolesCatalogos = "Administrador,Supervisor,RecursosHumanos";

    /// <summary>
    /// Devuelve todos los roles definidos en el sistema.
    /// </summary>
    public static readonly string[] TodosLosRoles =
    [
        Administrador,
        Supervisor,
        GestorPrestamos,
        RecursosHumanos,
        Consultor,
        Empleado
    ];
}

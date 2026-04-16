namespace PrestamosUcayNet.Application.DTOs;

public class CapacidadPagoResultDto
{
    public decimal SueldoNeto { get; set; }
    public decimal CuotaMaxPermitida { get; set; }
    public decimal DescuentosActivos { get; set; }
    public decimal CapacidadDisponible { get; set; }
    public decimal CuotaEstimadaProyectada { get; set; }
    public bool EsAprobado { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

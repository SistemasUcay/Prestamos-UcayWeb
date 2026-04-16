using System.ComponentModel.DataAnnotations;

namespace PrestamosUcayNet.Application.DTOs;

public class CapacidadPagoDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int EmpleadoId { get; set; }

    public int FondeadoraId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CondicionCreditoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal MontoSolicitado { get; set; }

    [Required]
    [Range(1, 360)]
    public int PlazoMeses { get; set; }
}

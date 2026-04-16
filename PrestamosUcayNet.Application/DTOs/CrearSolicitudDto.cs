using System.ComponentModel.DataAnnotations;

namespace PrestamosUcayNet.Application.DTOs;

public class CrearSolicitudDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El EmpleadoId es requerido.")]
    public int EmpleadoId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La CondicionCreditoId es requerida.")]
    public int CondicionCreditoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto solicitado debe ser mayor a cero.")]
    public decimal MontoSolicitado { get; set; }

    [Required]
    [Range(1, 360, ErrorMessage = "El plazo debe ser entre 1 y 360 meses.")]
    public int PlazoMeses { get; set; }

    [MaxLength(500)]
    public string Motivo { get; set; } = string.Empty;
}

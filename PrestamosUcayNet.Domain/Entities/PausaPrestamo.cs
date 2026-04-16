using System;

namespace PrestamosUcayNet.Domain.Entities;

public partial class PausaPrestamo
{
    public int Id { get; set; }
    
    public int PrestamoId { get; set; }
    
    public DateTime FechaInicio { get; set; }
    
    public int DuracionMeses { get; set; }
    
    public string Motivo { get; set; } = null!;
    
    public int? AutorizadoPorUserId { get; set; }
    
    public DateTime FechaFin { get; set; }
    
    public bool Activa { get; set; }

    public virtual Prestamo Prestamo { get; set; } = null!;
}

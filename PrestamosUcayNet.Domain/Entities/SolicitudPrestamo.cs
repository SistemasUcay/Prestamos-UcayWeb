using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class SolicitudPrestamo
{
    public int Id { get; set; }
    
    public int EmpleadoId { get; set; }
    
    public int EmpresaEmpleadoraId { get; set; }
    
    public int FondeadoraId { get; set; }
    
    public int CondicionCreditoId { get; set; }
    
    public decimal MontoSolicitado { get; set; }
    
    public int PlazoMeses { get; set; }
    
    public string Motivo { get; set; } = null!;
    
    public string EstadoSolicitud { get; set; } = null!;
    
    public DateTime FechaSolicitud { get; set; }
    
    public string? MotivoRechazo { get; set; }
    
    public string? MotivoCancelacion { get; set; }
    
    public int? AprobadoPorUserId { get; set; }

    public virtual Empleado Empleado { get; set; } = null!;
    public virtual DatosEmpresa EmpresaEmpleadora { get; set; } = null!;
    public virtual DatosEmpresa Fondeadora { get; set; } = null!;
    public virtual CondicionCredito CondicionCredito { get; set; } = null!;
    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}

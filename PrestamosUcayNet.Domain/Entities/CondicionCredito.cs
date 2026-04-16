using System;
using System.Collections.Generic;

namespace PrestamosUcayNet.Domain.Entities;

public partial class CondicionCredito
{
    public int Id { get; set; }
    
    // Fondeadora a la que pertenece esta condición (Empresa o Caja de Ahorro)
    public int FondeadoraId { get; set; }
    
    public string NombreProducto { get; set; } = null!;
    
    public decimal TasaInteresMensual { get; set; }
    
    public int PlazoMaximoMeses { get; set; }
    
    public decimal MontoMaximo { get; set; }
    
    public decimal MontoMinimo { get; set; }
    
    public bool Activo { get; set; }

    public virtual DatosEmpresa Fondeadora { get; set; } = null!;
    public virtual ICollection<SolicitudPrestamo> Solicitudes { get; set; } = new List<SolicitudPrestamo>();
}

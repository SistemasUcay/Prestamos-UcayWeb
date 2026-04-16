using PrestamosUcayNet.Domain.Enums;

namespace PrestamosUcayNet.Application.DTOs;

public class DepartamentoDto
{
    public int DepartamentoId { get; set; }
    public string Descripcion { get; set; } = null!;
    public int CantidadEmpleados { get; set; }
}

public class PuestoDto
{
    public int PuestoId { get; set; }
    public string Descripcion { get; set; } = null!;
    public int CantidadEmpleados { get; set; }
}

public class TipoPagoDto
{
    public int TipoPagoId { get; set; }
    public string Descripcion { get; set; } = null!;
    public int IntervaloDiario { get; set; }
    public int IntervaloMensual { get; set; }
    public int IntervaloAnual { get; set; }
    public int FrecuenciaAnual { get; set; }
    public int CantidadAsociados { get; set; }
}

public class TipoPrestamoDto
{
    public int TipoPrestamoId { get; set; }
    public string Descripcion { get; set; } = null!;
    public ClasificacionTipoPrestamo Clasificacion { get; set; }
    public int CantidadPrestamos { get; set; }
}

public class CondicionCreditoDto
{
    public int Id { get; set; }
    public int FondeadoraId { get; set; }
    public string NombreFondeadora { get; set; } = null!;
    public string NombreProducto { get; set; } = null!;
    public decimal TasaInteresMensual { get; set; }
    public int PlazoMaximoMeses { get; set; }
    public decimal MontoMinimo { get; set; }
    public decimal MontoMaximo { get; set; }
}

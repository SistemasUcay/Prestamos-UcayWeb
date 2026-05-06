using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para Prestamo → PrestamoListDto / PrestamoDetalleDto
/// </summary>
public class PrestamoProfile : Profile
{
    public PrestamoProfile()
    {
        CreateMap<Prestamo, PrestamoListDto>()
            .ForMember(d => d.NombreEmpleado, o => o.MapFrom(s =>
                s.Empleado != null
                    ? $"{s.Empleado.Nombres} {s.Empleado.ApellidoPaterno} {s.Empleado.ApellidoMaterno}".Trim()
                    : "N/A"))
            .ForMember(d => d.NombreFondeadora, o => o.MapFrom(s =>
                s.FuenteFondeo != null ? s.FuenteFondeo.RazonSocial : "N/A"))
            .ForMember(d => d.TipoPrestamoDescripcion, o => o.MapFrom(s =>
                s.TipoPrestamo != null ? s.TipoPrestamo.Descripcion : "N/A"))
            .ForMember(d => d.TipoPagoDescripcion, o => o.MapFrom(s =>
                s.TipoPago != null ? s.TipoPago.Descripcion : "N/A"));

        CreateMap<Prestamo, PrestamoDetalleDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.PrestamoId))
            .ForMember(d => d.EmpleadoId, o => o.MapFrom(s => s.EmpleadoId))
            .ForMember(d => d.FuenteFondeoId, o => o.MapFrom(s => s.FuenteFondeoId))
            .ForMember(d => d.TipoPrestamoId, o => o.MapFrom(s => s.TipoPrestamoId))
            .ForMember(d => d.TipoPagoId, o => o.MapFrom(s => s.TipoPagoId))
            .ForMember(d => d.NombreEmpleado, o => o.MapFrom(s =>
                s.Empleado != null
                    ? $"{s.Empleado.Nombres} {s.Empleado.ApellidoPaterno} {s.Empleado.ApellidoMaterno}".Trim()
                    : "N/A"))
            .ForMember(d => d.NombreFondeadora, o => o.MapFrom(s =>
                s.FuenteFondeo != null ? s.FuenteFondeo.RazonSocial : "N/A"))
            .ForMember(d => d.TipoPrestamoDescripcion, o => o.MapFrom(s =>
                s.TipoPrestamo != null ? s.TipoPrestamo.Descripcion : "N/A"))
            .ForMember(d => d.TipoPagoDescripcion, o => o.MapFrom(s =>
                s.TipoPago != null ? s.TipoPago.Descripcion : "N/A"))
            .ForMember(d => d.FechaDeposito, o => o.MapFrom(s => s.FechaDeposito))
            .ForMember(d => d.TablaAmortizacion, o => o.MapFrom(s =>
                s.PrestamosAmortizaciones.OrderBy(a => a.NumeroPago)))
            .ForMember(d => d.PrestamosDirectos, o => o.MapFrom(s => s.PrestamosDirectos));

        CreateMap<PrestamosAmortizacione, AmortizacionDto>()
            .ForMember(d => d.ImporteCapital, o => o.MapFrom(s => s.ImportePagoCapital))
            .ForMember(d => d.ImporteInteres, o => o.MapFrom(s => s.ImportePagoInteres))
            .ForMember(d => d.PagoExtra, o => o.MapFrom(s => s.PagoExtraCapital))
            .ForMember(d => d.Pagado, o => o.MapFrom(s => s.StatusPago));

        CreateMap<PrestamoDirecto, PrestamoDirectoDto>();
    }
}

using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

/// <summary>
/// Perfiles AutoMapper para SolicitudPrestamo y sus DTOs.
/// Las propiedades de nombre provienen de entidades de navegación que
/// SolicitudesService carga con Include() antes de llamar a Map().
/// </summary>
public class SolicitudProfile : Profile
{
    public SolicitudProfile()
    {
        // SolicitudPrestamo → SolicitudDetalleDto
        CreateMap<SolicitudPrestamo, SolicitudDetalleDto>()
            .ForMember(d => d.NombreEmpleado,
                opt => opt.MapFrom(s => s.Empleado != null
                    ? $"{s.Empleado.Nombres} {s.Empleado.ApellidoPaterno} {s.Empleado.ApellidoMaterno}".Trim()
                    : "N/A"))
            .ForMember(d => d.NombreEmpresaEmpleadora,
                opt => opt.MapFrom(s => s.EmpresaEmpleadora != null ? s.EmpresaEmpleadora.RazonSocial : "N/A"))
            .ForMember(d => d.NombreFondeadora,
                opt => opt.MapFrom(s => s.Fondeadora != null ? s.Fondeadora.RazonSocial : "N/A"))
            .ForMember(d => d.NombreProducto,
                opt => opt.MapFrom(s => s.CondicionCredito != null ? s.CondicionCredito.NombreProducto : "N/A"));

        // CrearSolicitudDto → SolicitudPrestamo
        // EmpresaEmpleadoraId, FondeadoraId, EstadoSolicitud y FechaSolicitud
        // se asignan programáticamente en el servicio, no desde el DTO.
        CreateMap<CrearSolicitudDto, SolicitudPrestamo>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.EmpresaEmpleadoraId, opt => opt.Ignore())
            .ForMember(d => d.FondeadoraId, opt => opt.Ignore())
            .ForMember(d => d.EstadoSolicitud, opt => opt.Ignore())
            .ForMember(d => d.FechaSolicitud, opt => opt.Ignore())
            .ForMember(d => d.MotivoRechazo, opt => opt.Ignore())
            .ForMember(d => d.MotivoCancelacion, opt => opt.Ignore())
            .ForMember(d => d.AprobadoPorUserId, opt => opt.Ignore())
            .ForMember(d => d.Empleado, opt => opt.Ignore())
            .ForMember(d => d.EmpresaEmpleadora, opt => opt.Ignore())
            .ForMember(d => d.Fondeadora, opt => opt.Ignore())
            .ForMember(d => d.CondicionCredito, opt => opt.Ignore())
            .ForMember(d => d.Prestamos, opt => opt.Ignore());
    }
}

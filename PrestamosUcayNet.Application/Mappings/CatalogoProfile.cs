using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

/// <summary>
/// Perfiles AutoMapper para los catálogos del sistema.
/// </summary>
public class CatalogoProfile : Profile
{
    public CatalogoProfile()
    {
        // Departamento ↔ DepartamentoDto
        CreateMap<Departamento, DepartamentoDto>()
            .ForMember(d => d.Descripcion,
                opt => opt.MapFrom(s => s.Descripcion ?? string.Empty))
            .ForMember(d => d.CantidadEmpleados,
                opt => opt.MapFrom(s => s.Empleados.Count));
        CreateMap<DepartamentoDto, Departamento>()
            .ForMember(d => d.Empleados, opt => opt.Ignore())
            .ForMember(d => d.DepartamentoId, opt => opt.Condition(src => src.DepartamentoId > 0));

        // Puesto ↔ PuestoDto
        CreateMap<Puesto, PuestoDto>()
            .ForMember(d => d.Descripcion,
                opt => opt.MapFrom(s => s.Descripcion ?? string.Empty))
            .ForMember(d => d.CantidadEmpleados,
                opt => opt.MapFrom(s => s.Empleados.Count));
        CreateMap<PuestoDto, Puesto>()
            .ForMember(d => d.Empleados, opt => opt.Ignore())
            .ForMember(d => d.PuestoId, opt => opt.Condition(src => src.PuestoId > 0));

        // TiposPago ↔ TipoPagoDto
        CreateMap<TiposPago, TipoPagoDto>()
            .ForMember(d => d.CantidadAsociados,
                opt => opt.MapFrom(s => s.Empleados.Count + s.Prestamos.Count));
        CreateMap<TipoPagoDto, TiposPago>()
            .ForMember(d => d.Empleados, opt => opt.Ignore())
            .ForMember(d => d.Prestamos, opt => opt.Ignore())
            .ForMember(d => d.TipoPagoId, opt => opt.Condition(src => src.TipoPagoId > 0));

        // CondicionCredito → CondicionCreditoDto
        // NombreFondeadora proviene de la navegación Fondeadora
        CreateMap<CondicionCredito, CondicionCreditoDto>()
            .ForMember(d => d.NombreFondeadora,
                opt => opt.MapFrom(s => s.Fondeadora != null ? s.Fondeadora.RazonSocial : string.Empty));

        // TipoPrestamo ↔ TipoPrestamoDto
        CreateMap<TipoPrestamo, TipoPrestamoDto>()
            .ForMember(d => d.CantidadPrestamos,
                opt => opt.MapFrom(s => s.Prestamos.Count));
        CreateMap<TipoPrestamoDto, TipoPrestamo>()
            .ForMember(d => d.Prestamos, opt => opt.Ignore())
            .ForMember(d => d.TipoPrestamoId, opt => opt.Condition(src => src.TipoPrestamoId > 0));
    }
}

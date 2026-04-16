using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

/// <summary>
/// Perfiles AutoMapper para la entidad Empleado y sus DTOs.
/// </summary>
public class EmpleadoProfile : Profile
{
    public EmpleadoProfile()
    {
        // Empleado → EmpleadoListDto
        // Resuelve propiedades de navegación: Empresa, Departamento, Puesto
        CreateMap<Empleado, EmpleadoListDto>()
            .ForMember(d => d.EmpresaNombre,
                opt => opt.MapFrom(s => s.Empresa != null ? s.Empresa.RazonSocial : string.Empty))
            .ForMember(d => d.DepartamentoNombre,
                opt => opt.MapFrom(s => s.Departamento != null ? s.Departamento.Descripcion : string.Empty))
            .ForMember(d => d.PuestoNombre,
                opt => opt.MapFrom(s => s.Puesto != null ? s.Puesto.Descripcion : string.Empty));

        // Empleado → EmpleadoDetailDto (hereda de EmpleadoListDto, reutiliza la configuración base)
        CreateMap<Empleado, EmpleadoDetailDto>()
            .IncludeBase<Empleado, EmpleadoListDto>();

        // EmpleadoCreateUpdateDto → Empleado (creación y actualización)
        // Nacimiento es DateTime? en el DTO y DateTime en la entidad; AutoMapper lo resuelve
        // automáticamente si el valor no es null. Para el caso nulo, el servicio valida antes.
        CreateMap<EmpleadoCreateUpdateDto, Empleado>()
            .ForMember(d => d.EmpleadoId, opt => opt.Ignore())
            .ForMember(d => d.Nacimiento,
                opt => opt.MapFrom(s => s.Nacimiento ?? DateTime.MinValue));
    }
}

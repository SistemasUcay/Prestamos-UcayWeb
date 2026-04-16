using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

/// <summary>
/// Perfiles AutoMapper para la entidad DatosEmpresa y sus DTOs.
/// </summary>
public class EmpresaProfile : Profile
{
    public EmpresaProfile()
    {
        // DatosEmpresa → EmpresaListDto
        CreateMap<DatosEmpresa, EmpresaListDto>();

        // DatosEmpresa → EmpresaDetailDto (hereda de EmpresaListDto)
        CreateMap<DatosEmpresa, EmpresaDetailDto>()
            .IncludeBase<DatosEmpresa, EmpresaListDto>();

        // EmpresaCreateUpdateDto → DatosEmpresa
        // NumEmpresa es clave autogenerada, se ignora al crear; al actualizar se
        // aplica sobre una entidad ya rastreada usando Map(dto, entity).
        CreateMap<EmpresaCreateUpdateDto, DatosEmpresa>()
            .ForMember(d => d.NumEmpresa, opt => opt.Ignore());
    }
}

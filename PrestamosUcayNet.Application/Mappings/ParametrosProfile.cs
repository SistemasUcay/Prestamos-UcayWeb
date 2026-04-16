using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

/// <summary>
/// Perfiles AutoMapper para los parámetros generales del sistema.
/// </summary>
public class ParametrosProfile : Profile
{
    public ParametrosProfile()
    {
        // ParametrosGenerale → ParametrosDto
        CreateMap<ParametrosGenerale, ParametrosDto>();

        // ParametrosDto → ParametrosGenerale
        // Al guardar, la entidad ya existe rastreada; ParametrosGeneralesId no se sobreescribe.
        CreateMap<ParametrosDto, ParametrosGenerale>()
            .ForMember(d => d.ParametrosGeneralesId, opt => opt.Ignore());
    }
}

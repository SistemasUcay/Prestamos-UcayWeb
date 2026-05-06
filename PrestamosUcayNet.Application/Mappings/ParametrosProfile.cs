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
        CreateMap<ParametrosGenerale, ParametrosDto>();

        // Al guardar se mapea todo incluyendo Anio (PK del registro)
        CreateMap<ParametrosDto, ParametrosGenerale>();
    }
}

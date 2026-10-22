using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Application.Mappings;

public class VacacionesProfile : Profile
{
    public VacacionesProfile()
    {
        CreateMap<PeriodoVacacional, PeriodoVacacionalDto>()
            .ForMember(d => d.EmpleadoNombre, opt => opt.MapFrom(s =>
                s.Empleado != null
                    ? $"{s.Empleado.Nombres} {s.Empleado.ApellidoPaterno} {s.Empleado.ApellidoMaterno}".Trim()
                    : string.Empty));

        CreateMap<SolicitudVacaciones, SolicitudVacacionesListDto>()
            .ForMember(d => d.EmpleadoNombre, opt => opt.MapFrom(s =>
                s.Empleado != null
                    ? $"{s.Empleado.Nombres} {s.Empleado.ApellidoPaterno} {s.Empleado.ApellidoMaterno}".Trim()
                    : string.Empty))
            .ForMember(d => d.NumeroNomina, opt => opt.MapFrom(s => s.Empleado != null ? s.Empleado.NumeroNomina : 0))
            .ForMember(d => d.EmpresaNombre, opt => opt.MapFrom(s => s.Empleado != null && s.Empleado.Empresa != null ? s.Empleado.Empresa.RazonSocial : string.Empty))
            .ForMember(d => d.DepartamentoNombre, opt => opt.MapFrom(s => s.Empleado != null && s.Empleado.Departamento != null ? s.Empleado.Departamento.Descripcion : string.Empty))
            .ForMember(d => d.PuestoNombre, opt => opt.MapFrom(s => s.Empleado != null && s.Empleado.Puesto != null ? s.Empleado.Puesto.Descripcion : string.Empty))
            .ForMember(d => d.FechaIngresoEmpleado, opt => opt.MapFrom(s => s.Empleado != null ? s.Empleado.FechaIngreso : null))
            .ForMember(d => d.AnioPeriodo, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? s.PeriodoVacacional.Anio : 0))
            .ForMember(d => d.AntiguedadAnios, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? s.PeriodoVacacional.AntiguedadAnios : 0))
            .ForMember(d => d.FechaInicioPeriodoGenerador, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? (DateTime?)s.PeriodoVacacional.FechaInicioPeriodo : null))
            .ForMember(d => d.FechaFinPeriodoGenerador, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? (DateTime?)s.PeriodoVacacional.FechaFinPeriodo : null))
            .ForMember(d => d.FechaFinPeriodoGoce, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? s.PeriodoVacacional.FechaVencimiento : null));

        CreateMap<SolicitudVacaciones, SolicitudVacacionesDetailDto>()
            .IncludeBase<SolicitudVacaciones, SolicitudVacacionesListDto>()
            .ForMember(d => d.DiasCorrespondientesPeriodo, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? s.PeriodoVacacional.DiasCorrespondientes : 0))
            .ForMember(d => d.DiasDisfrutadosPeriodo, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? s.PeriodoVacacional.DiasDisfrutados : 0))
            .ForMember(d => d.DiasPendientesPeriodo, opt => opt.MapFrom(s => s.PeriodoVacacional != null ? s.PeriodoVacacional.DiasPendientes : 0));
    }
}

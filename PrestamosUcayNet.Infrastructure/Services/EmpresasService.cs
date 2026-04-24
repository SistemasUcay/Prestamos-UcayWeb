using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

public class EmpresasService : IEmpresasService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;

    public EmpresasService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmpresaListDto>> ObtenerTodasAsync()
    {
        return await _context.DatosEmpresas
            .OrderBy(e => e.RazonSocial)
            .ProjectTo<EmpresaListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EmpresaDetailDto?> ObtenerPorIdAsync(int numEmpresa)
    {
        var e = await _context.DatosEmpresas.FindAsync(numEmpresa);
        return _mapper.Map<EmpresaDetailDto>(e);
    }

    public async Task<(bool Success, string? Error, int Id)> CrearAsync(EmpresaCreateUpdateDto dto)
    {
        try
        {
            if (await _context.DatosEmpresas.AnyAsync(x => x.RazonSocial.ToLower() == dto.RazonSocial.ToLower()))
                return (false, "Ya existe una empresa con esa Razón Social.", 0);

            if (await _context.DatosEmpresas.AnyAsync(x => x.Rfc == dto.Rfc))
                return (false, "Ya existe una empresa con ese RFC.", 0);

            var empresa = _mapper.Map<DatosEmpresa>(dto);

            _context.DatosEmpresas.Add(empresa);
            await _context.SaveChangesAsync();

            return (true, null, empresa.NumEmpresa);
        }
        catch (Exception ex)
        {
            return (false, $"Error al crear empresa: {ex.Message}", 0);
        }
    }

    public async Task<(bool Success, string? Error)> ActualizarAsync(int numEmpresa, EmpresaCreateUpdateDto dto)
    {
        try
        {
            var empresa = await _context.DatosEmpresas.FindAsync(numEmpresa);
            if (empresa == null) return (false, "Empresa no encontrada.");

            if (await _context.DatosEmpresas.AnyAsync(x => x.RazonSocial.ToLower() == dto.RazonSocial.ToLower() && x.NumEmpresa != numEmpresa))
                return (false, "Ya existe otra empresa con esa Razón Social.");

            if (await _context.DatosEmpresas.AnyAsync(x => x.Rfc == dto.Rfc && x.NumEmpresa != numEmpresa))
                return (false, "Ya existe otra empresa con ese RFC.");

            _mapper.Map(dto, empresa);

            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al actualizar empresa: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> EliminarAsync(int numEmpresa)
    {
        try
        {
            var empresa = await _context.DatosEmpresas.FindAsync(numEmpresa);
            if (empresa == null) return (false, "Empresa no encontrada.");

            // Validar si tiene empleados o préstamos asociados
            bool tieneRelaciones = await _context.Empleados.AnyAsync(x => x.EmpresaId == numEmpresa) ||
                                  await _context.Prestamos.AnyAsync(x => x.FuenteFondeoId == numEmpresa);

            if (tieneRelaciones)
                return (false, "No se puede eliminar la empresa porque tiene empleados o préstamos asociados. Considere desactivarla.");

            _context.DatosEmpresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar empresa: {ex.Message}");
        }
    }
}

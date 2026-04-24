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

public class EmpleadosService : IEmpleadosService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;

    public EmpleadosService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmpleadoListDto>> ObtenerTodosAsync(int? empresaId = null)
    {
        var query = _context.Empleados.AsQueryable();

        if (empresaId.HasValue)
            query = query.Where(x => x.EmpresaId == empresaId.Value);

        return await query
            .OrderBy(x => x.ApellidoPaterno)
            .ThenBy(x => x.ApellidoMaterno)
            .ProjectTo<EmpleadoListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EmpleadoDetailDto?> ObtenerPorIdAsync(int empleadoId)
    {
        var x = await _context.Empleados
            .Include(e => e.Empresa)
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .FirstOrDefaultAsync(e => e.EmpleadoId == empleadoId);

        return _mapper.Map<EmpleadoDetailDto>(x);
    }

    public async Task<(bool Success, string? Error, int Id)> CrearAsync(EmpleadoCreateUpdateDto dto)
    {
        try
        {
            if (await _context.Empleados.AnyAsync(x => x.NumeroNomina == dto.NumeroNomina))
                return (false, "Ya existe un empleado con ese número de nómina.", 0);

            if (await _context.Empleados.AnyAsync(x => x.Rfc == dto.Rfc))
                return (false, "Ya existe un empleado con ese RFC.", 0);

            var empleado = _mapper.Map<Empleado>(dto);

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return (true, null, empleado.EmpleadoId);
        }
        catch (Exception ex)
        {
            return (false, $"Error al crear empleado: {ex.Message}", 0);
        }
    }

    public async Task<(bool Success, string? Error)> ActualizarAsync(int empleadoId, EmpleadoCreateUpdateDto dto)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(empleadoId);
            if (empleado == null) return (false, "Empleado no encontrado.");

            if (await _context.Empleados.AnyAsync(x => x.NumeroNomina == dto.NumeroNomina && x.EmpleadoId != empleadoId))
                return (false, "Ya existe otro empleado con ese número de nómina.");

            if (await _context.Empleados.AnyAsync(x => x.Rfc == dto.Rfc && x.EmpleadoId != empleadoId))
                return (false, "Ya existe otro empleado con ese RFC.");

            _mapper.Map(dto, empleado);

            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al actualizar empleado: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? Error)> EliminarAsync(int empleadoId)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(empleadoId);
            if (empleado == null) return (false, "Empleado no encontrado.");

            // Validar si tiene préstamos
            if (await _context.Prestamos.AnyAsync(x => x.EmpleadoId == empleadoId))
            {
                return (false, "No se puede eliminar el empleado porque tiene préstamos asociados.");
            }

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar empleado: {ex.Message}");
        }
    }
}

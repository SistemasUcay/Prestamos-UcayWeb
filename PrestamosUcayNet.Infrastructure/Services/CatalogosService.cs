using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Infrastructure.Data;
using PrestamosUcayNet.Domain.Entities;

namespace PrestamosUcayNet.Infrastructure.Services;

public class CatalogosService : ICatalogosService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;

    public CatalogosService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // --- Departamentos ---

    public async Task<IEnumerable<DepartamentoDto>> ObtenerDepartamentosAsync()
    {
        return await _context.Departamentos
            .OrderBy(d => d.Descripcion)
            .ProjectTo<DepartamentoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<DepartamentoDto?> ObtenerDepartamentoPorIdAsync(int id)
    {
        var d = await _context.Departamentos.FindAsync(id);
        return _mapper.Map<DepartamentoDto>(d);
    }

    public async Task<bool> CrearDepartamentoAsync(DepartamentoDto dto)
    {
        if (await ExisteDepartamentoAsync(dto.Descripcion)) return false;

        var departamento = _mapper.Map<Departamento>(dto);
        _context.Departamentos.Add(departamento);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ActualizarDepartamentoAsync(DepartamentoDto dto)
    {
        var d = await _context.Departamentos.FindAsync(dto.DepartamentoId);
        if (d == null) return false;

        if (await _context.Departamentos.AnyAsync(x => x.Descripcion == dto.Descripcion && x.DepartamentoId != dto.DepartamentoId))
            return false;

        _mapper.Map(dto, d);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarDepartamentoAsync(int id)
    {
        var d = await _context.Departamentos.FindAsync(id);
        if (d == null) return false;

        // Verificar si tiene empleados asociados (FK constraint)
        if (await _context.Empleados.AnyAsync(e => e.DepartamentoId == id))
            return false;

        _context.Departamentos.Remove(d);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ExisteDepartamentoAsync(string descripcion)
    {
        return await _context.Departamentos.AnyAsync(d => d.Descripcion == descripcion);
    }

    // --- Puestos ---

    public async Task<IEnumerable<PuestoDto>> ObtenerPuestosAsync()
    {
        return await _context.Puestos
            .OrderBy(p => p.Descripcion)
            .ProjectTo<PuestoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PuestoDto?> ObtenerPuestoPorIdAsync(int id)
    {
        var p = await _context.Puestos.FindAsync(id);
        return _mapper.Map<PuestoDto>(p);
    }

    public async Task<bool> CrearPuestoAsync(PuestoDto dto)
    {
        if (await ExistePuestoAsync(dto.Descripcion)) return false;

        var puesto = _mapper.Map<Puesto>(dto);
        _context.Puestos.Add(puesto);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ActualizarPuestoAsync(PuestoDto dto)
    {
        var p = await _context.Puestos.FindAsync(dto.PuestoId);
        if (p == null) return false;

        if (await _context.Puestos.AnyAsync(x => x.Descripcion == dto.Descripcion && x.PuestoId != dto.PuestoId))
            return false;

        _mapper.Map(dto, p);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarPuestoAsync(int id)
    {
        var p = await _context.Puestos.FindAsync(id);
        if (p == null) return false;

        if (await _context.Empleados.AnyAsync(e => e.PuestoId == id))
            return false;

        _context.Puestos.Remove(p);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ExistePuestoAsync(string descripcion)
    {
        return await _context.Puestos.AnyAsync(p => p.Descripcion == descripcion);
    }

    public async Task<IEnumerable<TipoPagoDto>> ObtenerTiposPagoAsync()
    {
        return await _context.TiposPagos
            .OrderBy(t => t.Descripcion)
            .ProjectTo<TipoPagoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TipoPagoDto?> ObtenerTipoPagoPorIdAsync(int id)
    {
        var t = await _context.TiposPagos.FindAsync(id);
        return t is null ? null : _mapper.Map<TipoPagoDto>(t);
    }

    public async Task<bool> CrearTipoPagoAsync(TipoPagoDto dto)
    {
        if (await _context.TiposPagos.AnyAsync(t => t.Descripcion == dto.Descripcion))
            return false;

        var tipoPago = _mapper.Map<TiposPago>(dto);
        _context.TiposPagos.Add(tipoPago);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ActualizarTipoPagoAsync(TipoPagoDto dto)
    {
        var t = await _context.TiposPagos.FindAsync(dto.TipoPagoId);
        if (t is null) return false;

        if (await _context.TiposPagos.AnyAsync(x => x.Descripcion == dto.Descripcion && x.TipoPagoId != dto.TipoPagoId))
            return false;

        _mapper.Map(dto, t);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarTipoPagoAsync(int id)
    {
        var t = await _context.TiposPagos.FindAsync(id);
        if (t is null) return false;

        if (await _context.Empleados.AnyAsync(e => e.TipoPagoId == id))
            return false;

        if (await _context.Prestamos.AnyAsync(p => p.TipoPagoId == id))
            return false;

        _context.TiposPagos.Remove(t);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<CondicionCreditoDto>> ObtenerCondicionesCreditoActivasAsync()
    {
        return await _context.CondicionesCredito
            .Where(c => c.Activo)
            .Include(c => c.Fondeadora)
            .OrderBy(c => c.NombreProducto)
            .ProjectTo<CondicionCreditoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<CondicionCreditoDto>> ObtenerCondicionesCreditoPorFondeadoraAsync(int fondeadoraId)
    {
        return await _context.CondicionesCredito
            .Where(c => c.Activo && c.FondeadoraId == fondeadoraId)
            .Include(c => c.Fondeadora)
            .OrderBy(c => c.NombreProducto)
            .ProjectTo<CondicionCreditoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // --- Tipos de Préstamo ---

    public async Task<IEnumerable<TipoPrestamoDto>> ObtenerTiposPrestamoAsync()
    {
        return await _context.TipoPrestamos
            .OrderBy(t => t.Descripcion)
            .ProjectTo<TipoPrestamoDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TipoPrestamoDto?> ObtenerTipoPrestamoPorIdAsync(int id)
    {
        var t = await _context.TipoPrestamos.FindAsync(id);
        return t is null ? null : _mapper.Map<TipoPrestamoDto>(t);
    }

    public async Task<bool> CrearTipoPrestamoAsync(TipoPrestamoDto dto)
    {
        if (await _context.TipoPrestamos.AnyAsync(t => t.Descripcion == dto.Descripcion))
            return false;

        var tipoPrestamo = _mapper.Map<TipoPrestamo>(dto);
        _context.TipoPrestamos.Add(tipoPrestamo);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ActualizarTipoPrestamoAsync(TipoPrestamoDto dto)
    {
        var t = await _context.TipoPrestamos.FindAsync(dto.TipoPrestamoId);
        if (t is null) return false;

        if (await _context.TipoPrestamos.AnyAsync(x => x.Descripcion == dto.Descripcion && x.TipoPrestamoId != dto.TipoPrestamoId))
            return false;

        _mapper.Map(dto, t);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarTipoPrestamoAsync(int id)
    {
        var t = await _context.TipoPrestamos.FindAsync(id);
        if (t is null) return false;

        if (await _context.Prestamos.AnyAsync(p => p.TipoPrestamoId == id))
            return false;

        _context.TipoPrestamos.Remove(t);
        return await _context.SaveChangesAsync() > 0;
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using PrestamosUcayNet.Domain.Entities;
using PrestamosUcayNet.Infrastructure.Data;

namespace PrestamosUcayNet.Infrastructure.Services;

public class ParametrosService : IParametrosService
{
    private readonly UcayNetDbContext _context;
    private readonly IMapper _mapper;

    public ParametrosService(UcayNetDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ParametrosDto> ObtenerPorAnioAsync(int anio)
    {
        var p = await _context.ParametrosGenerales.FindAsync(anio);
        return _mapper.Map<ParametrosDto>(p ?? new ParametrosGenerale { Anio = anio });
    }

    public async Task<(bool Success, string? Error)> GuardarAsync(ParametrosDto dto)
    {
        try
        {
            var p = await _context.ParametrosGenerales.FindAsync(dto.Anio);

            if (p == null)
            {
                p = _mapper.Map<ParametrosGenerale>(dto);
                _context.ParametrosGenerales.Add(p);
            }
            else
            {
                _mapper.Map(dto, p);
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al guardar parámetros: {ex.Message}");
        }
    }
}

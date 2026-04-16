using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ParametrosDto> ObtenerAsync()
    {
        var p = await _context.ParametrosGenerales.FirstOrDefaultAsync();
        return _mapper.Map<ParametrosDto>(p ?? new ParametrosGenerale());
    }

    public async Task<(bool Success, string? Error)> GuardarAsync(ParametrosDto dto)
    {
        try
        {
            var p = await _context.ParametrosGenerales.FirstOrDefaultAsync();

            if (p == null)
            {
                p = new ParametrosGenerale();
                _context.ParametrosGenerales.Add(p);
            }

            _mapper.Map(dto, p);

            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error al guardar parámetros: {ex.Message}");
        }
    }
}

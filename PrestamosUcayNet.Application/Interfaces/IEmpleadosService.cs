using System.Collections.Generic;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface IEmpleadosService
{
    Task<IEnumerable<EmpleadoListDto>> ObtenerTodosAsync(int? empresaId = null);
    Task<EmpleadoDetailDto?> ObtenerPorIdAsync(int empleadoId);
    Task<(bool Success, string? Error, int Id)> CrearAsync(EmpleadoCreateUpdateDto dto);
    Task<(bool Success, string? Error)> ActualizarAsync(int empleadoId, EmpleadoCreateUpdateDto dto);
    Task<(bool Success, string? Error)> EliminarAsync(int empleadoId);
}

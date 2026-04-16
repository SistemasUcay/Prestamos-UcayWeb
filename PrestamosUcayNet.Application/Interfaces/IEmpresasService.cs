using System.Collections.Generic;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface IEmpresasService
{
    Task<IEnumerable<EmpresaListDto>> ObtenerTodasAsync();
    Task<EmpresaDetailDto?> ObtenerPorIdAsync(int numEmpresa);
    Task<(bool Success, string? Error, int Id)> CrearAsync(EmpresaCreateUpdateDto dto);
    Task<(bool Success, string? Error)> ActualizarAsync(int numEmpresa, EmpresaCreateUpdateDto dto);
    Task<(bool Success, string? Error)> EliminarAsync(int numEmpresa);
}

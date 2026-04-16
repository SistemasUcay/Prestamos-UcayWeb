using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface IParametrosService
{
    Task<ParametrosDto> ObtenerAsync();
    Task<(bool Success, string? Error)> GuardarAsync(ParametrosDto dto);
}

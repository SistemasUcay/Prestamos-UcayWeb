using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface IParametrosService
{
    /// <summary>Obtiene los parámetros del año indicado. Devuelve un DTO vacío con ese año si no existe registro.</summary>
    Task<ParametrosDto> ObtenerPorAnioAsync(int anio);
    /// <summary>Crea o actualiza los parámetros del año indicado en el DTO.</summary>
    Task<(bool Success, string? Error)> GuardarAsync(ParametrosDto dto);
}

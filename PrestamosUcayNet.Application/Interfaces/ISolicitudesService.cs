using System.Collections.Generic;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface ISolicitudesService
{
    Task<CapacidadPagoResultDto> EvaluarCapacidadDePagoAsync(CapacidadPagoDto dto);
    Task<int> CrearSolicitudAsync(CrearSolicitudDto dto);
    
    Task<SolicitudDetalleDto?> ObtenerPorIdAsync(int id);
    Task<List<SolicitudDetalleDto>> ListarTodasAsync();
    Task<List<SolicitudDetalleDto>> ListarPendientesAsync();
    Task<List<SolicitudDetalleDto>> ListarPorEstadoAsync(string estado);
    Task<List<SolicitudDetalleDto>> ListarPorEmpresaAsync(int empresaId);

    Task<bool> EditarSolicitudAsync(EditarSolicitudDto dto);
    Task<bool> AprobarSolicitudAsync(AprobarSolicitudDto dto);
    Task<bool> RechazarSolicitudAsync(RechazarSolicitudDto dto);
    Task<bool> CancelarSolicitudAsync(CancelarSolicitudDto dto);
}

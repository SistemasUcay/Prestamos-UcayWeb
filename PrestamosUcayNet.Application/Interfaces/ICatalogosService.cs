using System.Collections.Generic;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface ICatalogosService
{
    Task<IEnumerable<DepartamentoDto>> ObtenerDepartamentosAsync();
    Task<DepartamentoDto?> ObtenerDepartamentoPorIdAsync(int id);
    Task<bool> CrearDepartamentoAsync(DepartamentoDto dto);
    Task<bool> ActualizarDepartamentoAsync(DepartamentoDto dto);
    Task<bool> EliminarDepartamentoAsync(int id);
    Task<bool> ExisteDepartamentoAsync(string descripcion);

    Task<IEnumerable<PuestoDto>> ObtenerPuestosAsync();
    Task<PuestoDto?> ObtenerPuestoPorIdAsync(int id);
    Task<bool> CrearPuestoAsync(PuestoDto dto);
    Task<bool> ActualizarPuestoAsync(PuestoDto dto);
    Task<bool> EliminarPuestoAsync(int id);
    Task<bool> ExistePuestoAsync(string descripcion);

    Task<IEnumerable<TipoPagoDto>> ObtenerTiposPagoAsync();
    Task<TipoPagoDto?> ObtenerTipoPagoPorIdAsync(int id);
    Task<bool> CrearTipoPagoAsync(TipoPagoDto dto);
    Task<bool> ActualizarTipoPagoAsync(TipoPagoDto dto);
    Task<bool> EliminarTipoPagoAsync(int id);

    Task<IEnumerable<CondicionCreditoDto>> ObtenerCondicionesCreditoActivasAsync();
    Task<IEnumerable<CondicionCreditoDto>> ObtenerCondicionesCreditoPorFondeadoraAsync(int fondeadoraId);

    Task<IEnumerable<TipoPrestamoDto>> ObtenerTiposPrestamoAsync();
    Task<TipoPrestamoDto?> ObtenerTipoPrestamoPorIdAsync(int id);
    Task<bool> CrearTipoPrestamoAsync(TipoPrestamoDto dto);
    Task<bool> ActualizarTipoPrestamoAsync(TipoPrestamoDto dto);
    Task<bool> EliminarTipoPrestamoAsync(int id);
}

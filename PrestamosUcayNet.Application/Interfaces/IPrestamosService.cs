using System.Collections.Generic;
using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface IPrestamosService
{
    /// <summary>
    /// Crea un préstamo directamente, generando automáticamente la solicitud asociada
    /// </summary>
    Task<int> CrearPrestamoAsync(PrestamoCreateDto dto);

    /// <summary>
    /// Obtiene el detalle completo de un préstamo con tabla de amortización
    /// </summary>
    Task<PrestamoDetalleDto?> ObtenerDetalleAsync(int id);

    /// <summary>
    /// Lista todos los préstamos registrados
    /// </summary>
    Task<List<PrestamoListDto>> ListarTodosAsync();

    /// <summary>
    /// Lista préstamos filtrados por estado (A=Activo, P=Pagado, L=Liquidado, C=Cancelado, S=Suspendido)
    /// </summary>
    Task<List<PrestamoListDto>> ListarPorEstadoAsync(string estado);

    /// <summary>
    /// Lista préstamos de un empleado específico
    /// </summary>
    Task<List<PrestamoListDto>> ListarPorEmpleadoAsync(int empleadoId);

    /// <summary>
    /// Activa un préstamo a partir de una solicitud aprobada
    /// </summary>
    Task<bool> ActivarPrestamoAsync(ActivarPrestamoDto dto);

    /// <summary>
    /// Actualiza campos editables de un préstamo
    /// </summary>
    Task<bool> ActualizarPrestamoAsync(PrestamoUpdateDto dto);

    /// <summary>
    /// Cancela un préstamo activo
    /// </summary>
    Task<bool> CancelarPrestamoAsync(int prestamoId);

    /// <summary>
    /// Registra un pago al préstamo
    /// </summary>
    Task<bool> RegistrarPagoAsync(RegistrarPagoDto dto);

    /// <summary>
    /// Solicita una pausa de cuotas del préstamo
    /// </summary>
    Task<bool> SolicitarPausaAsync(SolicitudPausaDto dto);

    /// <summary>
    /// Simula la tabla de amortización para previsualización sin persistir
    /// </summary>
    AmortizacionPreviewResultDto SimularAmortizacion(SimularAmortizacionDto dto);
}

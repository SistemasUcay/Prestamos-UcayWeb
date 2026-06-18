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
    /// Evalúa una solicitud de préstamo (Aprobar, Rechazar o Cancelar)
    /// </summary>
    Task<bool> EvaluarPrestamoAsync(EvaluarPrestamoDto dto);

    /// <summary>
    /// Aprueba una solicitud de préstamo y la activa
    /// </summary>
    Task<bool> AprobarPrestamoAsync(int prestamoId, DateTime fechaEntrega, string metodoEntrega, string referencia, string? folio, string? notas);

    /// <summary>
    /// Rechaza una solicitud de préstamo
    /// </summary>
    Task<bool> RechazarPrestamoAsync(int prestamoId, string notas);

    /// <summary>
    /// Cancela una solicitud de préstamo en evaluación
    /// </summary>
    Task<bool> CancelarSolicitudAsync(int prestamoId, string notas);

    /// <summary>
    /// Devuelve una solicitud pendiente de entrega al estado en evaluación
    /// </summary>
    Task<bool> DevolverAEvaluacionAsync(int prestamoId);

    /// <summary>
    /// Simula la tabla de amortización para previsualización sin persistir
    /// </summary>
    AmortizacionPreviewResultDto SimularAmortizacion(SimularAmortizacionDto dto);

    /// <summary>
    /// Actualiza una solicitud existente (estado E) reemplazando todos sus datos,
    /// amortizaciones y préstamos directos. Nunca crea un registro nuevo.
    /// </summary>
    Task<bool> ActualizarSolicitudAsync(int prestamoId, PrestamoCreateDto dto);
}

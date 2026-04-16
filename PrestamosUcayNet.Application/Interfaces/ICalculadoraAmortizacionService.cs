using System.Threading.Tasks;
using PrestamosUcayNet.Application.DTOs;

namespace PrestamosUcayNet.Application.Interfaces;

public interface ICalculadoraAmortizacionService
{
    Task GenerarYGuardarTablaAmortizacionAsync(int prestamoId);
    Task RecalcularYGuardarPorPagoAnticipadoAsync(int prestamoId, decimal pagoExtra, bool mantenerCuotaReducirPlazo);
    Task PausarCuotasAsync(int prestamoId, int mesesPausa);

    /// <summary>
    /// Simula la tabla de amortización sin persistir, para previsualización en el formulario de captura
    /// </summary>
    AmortizacionPreviewResultDto SimularAmortizacion(SimularAmortizacionDto dto);
}

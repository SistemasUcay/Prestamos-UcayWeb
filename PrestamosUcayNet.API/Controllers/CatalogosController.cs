using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrestamosUcayNet.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogosService _catalogosService;

    public CatalogosController(ICatalogosService catalogosService)
    {
        _catalogosService = catalogosService;
    }

    // --- Departamentos ---

    [HttpGet("departamentos")]
    public async Task<ActionResult<IEnumerable<DepartamentoDto>>> GetDepartamentos()
    {
        return Ok(await _catalogosService.ObtenerDepartamentosAsync());
    }

    [HttpGet("departamentos/{id}")]
    public async Task<ActionResult<DepartamentoDto>> GetDepartamento(int id)
    {
        var d = await _catalogosService.ObtenerDepartamentoPorIdAsync(id);
        if (d == null) return NotFound();
        return Ok(d);
    }

    [HttpPost("departamentos")]
    public async Task<ActionResult> CreateDepartamento(DepartamentoDto dto)
    {
        var result = await _catalogosService.CrearDepartamentoAsync(dto);
        if (!result) return BadRequest("No se pudo crear el departamento. Verifique si ya existe.");
        return Ok();
    }

    [HttpPut("departamentos/{id}")]
    public async Task<ActionResult> UpdateDepartamento(int id, DepartamentoDto dto)
    {
        if (id != dto.DepartamentoId) return BadRequest();
        var result = await _catalogosService.ActualizarDepartamentoAsync(dto);
        if (!result) return BadRequest("No se pudo actualizar el departamento.");
        return Ok();
    }

    [HttpDelete("departamentos/{id}")]
    public async Task<ActionResult> DeleteDepartamento(int id)
    {
        var result = await _catalogosService.EliminarDepartamentoAsync(id);
        if (!result) return BadRequest("No se pudo eliminar el departamento. Verifique si tiene empleados asociados.");
        return Ok();
    }

    // --- Puestos ---

    [HttpGet("puestos")]
    public async Task<ActionResult<IEnumerable<PuestoDto>>> GetPuestos()
    {
        return Ok(await _catalogosService.ObtenerPuestosAsync());
    }

    [HttpGet("puestos/{id}")]
    public async Task<ActionResult<PuestoDto>> GetPuesto(int id)
    {
        var p = await _catalogosService.ObtenerPuestoPorIdAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost("puestos")]
    public async Task<ActionResult> CreatePuesto(PuestoDto dto)
    {
        var result = await _catalogosService.CrearPuestoAsync(dto);
        if (!result) return BadRequest("No se pudo crear el puesto. Verifique si ya existe.");
        return Ok();
    }

    [HttpPut("puestos/{id}")]
    public async Task<ActionResult> UpdatePuesto(int id, PuestoDto dto)
    {
        if (id != dto.PuestoId) return BadRequest();
        var result = await _catalogosService.ActualizarPuestoAsync(dto);
        if (!result) return BadRequest("No se pudo actualizar el puesto.");
        return Ok();
    }

    [HttpDelete("puestos/{id}")]
    public async Task<ActionResult> DeletePuesto(int id)
    {
        var result = await _catalogosService.EliminarPuestoAsync(id);
        if (!result) return BadRequest("No se pudo eliminar el puesto. Verifique si tiene empleados asociados.");
        return Ok();
    }

    [HttpGet("tipospago")]
    public async Task<ActionResult<IEnumerable<TipoPagoDto>>> GetTiposPago()
    {
        return Ok(await _catalogosService.ObtenerTiposPagoAsync());
    }

    [HttpGet("tipospago/{id}")]
    public async Task<ActionResult<TipoPagoDto>> GetTipoPago(int id)
    {
        var t = await _catalogosService.ObtenerTipoPagoPorIdAsync(id);
        if (t is null) return NotFound();
        return Ok(t);
    }

    [HttpPost("tipospago")]
    public async Task<ActionResult> CreateTipoPago(TipoPagoDto dto)
    {
        var result = await _catalogosService.CrearTipoPagoAsync(dto);
        if (!result) return BadRequest("No se pudo crear la frecuencia de pago. Verifique si ya existe.");
        return Ok();
    }

    [HttpPut("tipospago/{id}")]
    public async Task<ActionResult> UpdateTipoPago(int id, TipoPagoDto dto)
    {
        if (id != dto.TipoPagoId) return BadRequest();
        var result = await _catalogosService.ActualizarTipoPagoAsync(dto);
        if (!result) return BadRequest("No se pudo actualizar la frecuencia de pago.");
        return Ok();
    }

    [HttpDelete("tipospago/{id}")]
    public async Task<ActionResult> DeleteTipoPago(int id)
    {
        var result = await _catalogosService.EliminarTipoPagoAsync(id);
        if (!result) return BadRequest("No se pudo eliminar. Verifique que no tenga empleados o préstamos asociados.");
        return Ok();
    }

    // --- Condiciones de Crédito ---

    [HttpGet("condiciones-credito")]
    public async Task<ActionResult<IEnumerable<CondicionCreditoDto>>> GetCondicionesCredito()
    {
        return Ok(await _catalogosService.ObtenerCondicionesCreditoActivasAsync());
    }

    [HttpGet("condiciones-credito/fondeadora/{fondeadoraId}")]
    public async Task<ActionResult<IEnumerable<CondicionCreditoDto>>> GetCondicionesCreditoPorFondeadora(int fondeadoraId)
    {
        return Ok(await _catalogosService.ObtenerCondicionesCreditoPorFondeadoraAsync(fondeadoraId));
    }

    // --- Tipos de Préstamo ---

    [HttpGet("tipos-prestamo")]
    public async Task<ActionResult<IEnumerable<TipoPrestamoDto>>> GetTiposPrestamo()
    {
        return Ok(await _catalogosService.ObtenerTiposPrestamoAsync());
    }

    [HttpGet("tipos-prestamo/{id}")]
    public async Task<ActionResult<TipoPrestamoDto>> GetTipoPrestamo(int id)
    {
        var t = await _catalogosService.ObtenerTipoPrestamoPorIdAsync(id);
        if (t is null) return NotFound();
        return Ok(t);
    }

    [HttpPost("tipos-prestamo")]
    public async Task<ActionResult> CreateTipoPrestamo(TipoPrestamoDto dto)
    {
        var result = await _catalogosService.CrearTipoPrestamoAsync(dto);
        if (!result) return BadRequest("No se pudo crear el tipo de préstamo. Verifique si ya existe.");
        return Ok();
    }

    [HttpPut("tipos-prestamo/{id}")]
    public async Task<ActionResult> UpdateTipoPrestamo(int id, TipoPrestamoDto dto)
    {
        if (id != dto.TipoPrestamoId) return BadRequest();
        var result = await _catalogosService.ActualizarTipoPrestamoAsync(dto);
        if (!result) return BadRequest("No se pudo actualizar el tipo de préstamo.");
        return Ok();
    }

    [HttpDelete("tipos-prestamo/{id}")]
    public async Task<ActionResult> DeleteTipoPrestamo(int id)
    {
        var result = await _catalogosService.EliminarTipoPrestamoAsync(id);
        if (!result) return BadRequest("No se pudo eliminar. Verifique que no tenga préstamos asociados.");
        return Ok();
    }
}

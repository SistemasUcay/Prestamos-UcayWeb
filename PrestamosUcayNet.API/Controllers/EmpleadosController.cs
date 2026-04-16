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
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadosService _empleadosService;

    public EmpleadosController(IEmpleadosService empleadosService)
    {
        _empleadosService = empleadosService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpleadoListDto>>> Get([FromQuery] int? empresaId)
    {
        return Ok(await _empleadosService.ObtenerTodosAsync(empresaId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmpleadoDetailDto>> Get(int id)
    {
        var empleado = await _empleadosService.ObtenerPorIdAsync(id);
        if (empleado == null) return NotFound();
        return Ok(empleado);
    }

    [HttpPost]
    public async Task<ActionResult> Post(EmpleadoCreateUpdateDto dto)
    {
        var result = await _empleadosService.CrearAsync(dto);
        if (!result.Success) return BadRequest(result.Error);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, EmpleadoCreateUpdateDto dto)
    {
        var result = await _empleadosService.ActualizarAsync(id, dto);
        if (!result.Success) return BadRequest(result.Error);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _empleadosService.EliminarAsync(id);
        if (!result.Success) return BadRequest(result.Error);
        return NoContent();
    }
}

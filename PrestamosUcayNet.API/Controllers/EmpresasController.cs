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
public class EmpresasController : ControllerBase
{
    private readonly IEmpresasService _empresasService;

    public EmpresasController(IEmpresasService empresasService)
    {
        _empresasService = empresasService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmpresaListDto>>> Get()
    {
        return Ok(await _empresasService.ObtenerTodasAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmpresaDetailDto>> Get(int id)
    {
        var empresa = await _empresasService.ObtenerPorIdAsync(id);
        if (empresa == null) return NotFound();
        return Ok(empresa);
    }

    [HttpPost]
    public async Task<ActionResult> Post(EmpresaCreateUpdateDto dto)
    {
        var result = await _empresasService.CrearAsync(dto);
        if (!result.Success) return BadRequest(result.Error);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, EmpresaCreateUpdateDto dto)
    {
        var result = await _empresasService.ActualizarAsync(id, dto);
        if (!result.Success) return BadRequest(result.Error);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _empresasService.EliminarAsync(id);
        if (!result.Success) return BadRequest(result.Error);
        return NoContent();
    }
}

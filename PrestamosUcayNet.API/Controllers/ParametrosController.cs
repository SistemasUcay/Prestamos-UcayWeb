using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosUcayNet.Application.DTOs;
using PrestamosUcayNet.Application.Interfaces;
using System.Threading.Tasks;

namespace PrestamosUcayNet.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ParametrosController : ControllerBase
{
    private readonly IParametrosService _parametrosService;

    public ParametrosController(IParametrosService parametrosService)
    {
        _parametrosService = parametrosService;
    }

    [HttpGet("{anio:int}")]
    public async Task<ActionResult<ParametrosDto>> Get(int anio)
    {
        return Ok(await _parametrosService.ObtenerPorAnioAsync(anio));
    }

    [HttpPost]
    public async Task<ActionResult> Post(ParametrosDto dto)
    {
        var result = await _parametrosService.GuardarAsync(dto);
        if (!result.Success) return BadRequest(result.Error);
        return Ok();
    }
}

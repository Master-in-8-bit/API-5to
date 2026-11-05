using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteResponse>> Register([FromBody] RegisterClienteRequest request)
    {
        try
        {
            var result = await _clienteService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginClienteRequest request)
    {
        try
        {
            var result = await _clienteService.LoginAsync(request, "");
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClienteResponse>>> GetAll()
    {
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> GetById(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> Update(int id, [FromBody] UpdateClienteRequest request)
    {
        try
        {
            var cliente = await _clienteService.UpdateAsync(id, request);
            if (cliente is null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _clienteService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

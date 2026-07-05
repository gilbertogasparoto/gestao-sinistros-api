using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Api.DTOs.Cliente;
using gestao_sinistros_api.Application.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace gestao_sinistros_api.Application.Api.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _service;

        public ClienteController(ClienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetClientesParams query)
        {
            var clientes = await _service.GetAll(query);
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cliente = await _service.GetById(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpsertClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpsertClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _service.Update(id, dto);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cliente = await _service.Delete(id);
            if (!cliente) return NotFound();
            return Ok(cliente);
        }
    }
}
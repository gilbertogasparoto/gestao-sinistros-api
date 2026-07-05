using gestao_sinistros_api.Application.Api.DTOs.Sinistro;
using gestao_sinistros_api.Application.Api.Services;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace gestao_sinistros_api.Application.Api.Controllers
{
    [ApiController]
    [Route("api/sinistros")]
    public class SinistroController : ControllerBase
    {
        private readonly SinistroService _service;

        public SinistroController(SinistroService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetSinistrosParams query)
        {
            var result = await _service.GetAll(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var sinistro = await _service.GetById(id);
            if (sinistro == null) return NotFound();
            return Ok(sinistro);
        }

        [HttpGet("{id}/historico")]
        public async Task<IActionResult> GetHistorico(Guid id)
        {
            var historico = await _service.GetHistorico(id);
            return Ok(historico);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSinistroDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var sinistro = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = sinistro.Id }, sinistro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSinistroDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sinistro = await _service.Update(id, dto);
            if (sinistro == null) return NotFound();
            return Ok(sinistro);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusSinistroDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var sinistro = await _service.ChangeStatus(id, dto);
                if (sinistro == null) return NotFound();
                return Ok(sinistro);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }
    }
}

using gestao_sinistros_api.Application.Api.DTOs;
using gestao_sinistros_api.Application.Api.Services;
using gestao_sinistros_api.Application.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace gestao_sinistros_api.Application.Api.Controllers
{
    [ApiController]
    [Route("api/apolices")]
    public class ApoliceController : ControllerBase
    {
        private readonly ApoliceService _service;

        public ApoliceController(ApoliceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetApolicesParams query)
        {
            var result = await _service.GetAll(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var apolice = await _service.GetById(id);
            if (apolice == null) return NotFound();
            return Ok(apolice);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApoliceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var apolice = await _service.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = apolice.Id }, apolice);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApoliceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var apolice = await _service.Update(id, dto);
            if (apolice == null) return NotFound();
            return Ok(apolice);
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

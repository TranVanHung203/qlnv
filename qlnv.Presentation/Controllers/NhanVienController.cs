using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace qlnv.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienService _service;
        public NhanVienController(INhanVienService service) { _service = service; }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNhanVienDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var nv = await _service.GetByIdAsync(id);
            if (nv == null) return NotFound();
            return Ok(nv);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] string? ten = null, [FromQuery] string? sdt = null)
        {
            var result = await _service.GetPagedAsync(page, 8, ten, sdt);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNhanVienDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "Id mismatch" });
            var updated = await _service.UpdateAsync(dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}
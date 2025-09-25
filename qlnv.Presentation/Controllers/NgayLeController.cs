using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Entities.DTOs;
using System;
using System.Threading.Tasks;

namespace qlnv.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NgayLeController : ControllerBase
    {
        private readonly INgayLeService _service;

        public NgayLeController(INgayLeService service)
        {
            _service = service;
        }

        // GET api/NgayLe?page=1&pageSize=8&ten=Quoc
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string? ten = null)
        {
            try
            {
                var result = await _service.GetPagedAsync(page, pageSize, ten);
                if (result.TotalItems == 0)
                    return NotFound(new { message = "Không tìm thấy ngày lễ nào." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // GET api/NgayLe/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound(new { message = "Không tìm thấy ngày lễ" });
            return Ok(item);
        }

        // POST api/NgayLe
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNgayLeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // PUT api/NgayLe/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNgayLeDto dto)
        {
            if (id != dto.Id) return BadRequest(new { message = "Id không khớp" });
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            try
            {
                var result = await _service.UpdateAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // DELETE api/NgayLe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
    }
}

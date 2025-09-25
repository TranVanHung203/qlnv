using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Entities.DTOs;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace qlnv.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailThongBaoController : ControllerBase
    {
        private readonly IEmailThongBaoService _service;

        public EmailThongBaoController(IEmailThongBaoService service)
        {
            _service = service;
        }

        // GET api/EmailThongBao?page=1&pageSize=8&email=example
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 8, [FromQuery] string? email = null)
        {
            try
            {
                var result = await _service.GetPagedAsync(page, pageSize, email);
                if (result.TotalItems == 0)
                    return NotFound(new { message = "Không tìm thấy email thông báo nào." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // GET api/EmailThongBao/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound(new { message = "Không tìm thấy email thông báo" });
            return Ok(item);
        }

        // POST api/EmailThongBao
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmailThongBaoDto dto)
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }

        // PUT api/EmailThongBao/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmailThongBaoDto dto)
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        // DELETE api/EmailThongBao/5
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

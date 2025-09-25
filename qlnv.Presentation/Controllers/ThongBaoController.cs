using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Entities.DTOs;

namespace qlnv.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoService _service;

        public ThongBaoController(IThongBaoService service)
        {
            _service = service;
        }

        // GET api/ThongBao?page=1&pageSize=20&nhanVienId=1&emailNhan=abc&from=2025-09-01&to=2025-09-30
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ThongBaoFilterDto filter)
        {
            try
            {
                var paged = await _service.GetPagedAsync(filter.Page, filter.PageSize, filter.NhanVienId, filter.EmailNhan, filter.From, filter.To);
                if (paged.TotalItems == 0) return NotFound(new { message = "Không tìm thấy thông báo nào." });
                return Ok(paged);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống", detail = ex.Message });
            }
        }
    }
}

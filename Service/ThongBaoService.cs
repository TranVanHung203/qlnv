using Service.Contracts;
using Contracts;
using Entities.DTOs;

namespace Service
{
    public class ThongBaoService : IThongBaoService
    {
        private readonly IThongBaoRepository _repo;

        public ThongBaoService(IThongBaoRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<ThongBaoDto>> GetPagedAsync(int page, int pageSize, int? nhanVienId = null, string? emailNhan = null, DateTime? from = null, DateTime? to = null)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, nhanVienId, emailNhan, from, to);

            var items = paged.Items.Select(t => new ThongBaoDto
            {
                Id = t.Id,
                NhanVienId = t.NhanVienId,
                EmailNhan = t.EmailNhan,
                // Ensure the DateTime Kind is UTC so JSON serialization includes 'Z' and client can localize correctly
                NgayGui = DateTime.SpecifyKind(t.NgayGui, DateTimeKind.Utc),
                LyDo = t.LyDo,
                TenNhanVien = t.NhanVien?.Ten
            });

            return new PagedResult<ThongBaoDto>
            {
                Items = items,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalItems = paged.TotalItems,
                TotalPages = paged.TotalPages
            };
        }
    }
}

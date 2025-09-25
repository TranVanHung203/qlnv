using Contracts;
using Entities.DTOs;
using Entities.Models;
using Service.Contracts;

namespace Service
{
    public class NhanVienService : INhanVienService
    {
        private readonly INhanVienRepository _repo;
        public NhanVienService(INhanVienRepository repo) { _repo = repo; }

        public async Task<NhanVienDto> CreateAsync(CreateNhanVienDto dto)
        {
            var entity = new NhanVien
            {
                Ten = dto.Ten,
                Email = dto.Email,
                SoDienThoai = dto.SoDienThoai,
                DiaChi = dto.DiaChi,
                NgayVaoLam = dto.NgayVaoLam
            };
            var created = await _repo.CreateAsync(entity);
            return new NhanVienDto
            {
                Id = created.Id,
                Ten = created.Ten,
                Email = created.Email,
                SoDienThoai = created.SoDienThoai,
                DiaChi = created.DiaChi,
                NgayVaoLam = created.NgayVaoLam
            };
        }

        public async Task<NhanVienDto?> GetByIdAsync(int id)
        {
            var nv = await _repo.GetByIdAsync(id);
            return nv == null ? null : new NhanVienDto
            {
                Id = nv.Id,
                Ten = nv.Ten,
                Email = nv.Email,
                SoDienThoai = nv.SoDienThoai,
                DiaChi = nv.DiaChi,
                NgayVaoLam = nv.NgayVaoLam
            };
        }

        public async Task<PagedResult<NhanVienDto>> GetPagedAsync(int page, int pageSize, string? ten = null, string? sdt = null)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ten, sdt);
            return new PagedResult<NhanVienDto>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalItems = paged.TotalItems,
                TotalPages = paged.TotalPages,
                Items = paged.Items.Select(n => new NhanVienDto
                {
                    Id = n.Id,
                    Ten = n.Ten,
                    Email = n.Email,
                    SoDienThoai = n.SoDienThoai,
                    DiaChi = n.DiaChi,
                    NgayVaoLam = n.NgayVaoLam
                })
            };
        }

        public async Task<NhanVienDto> UpdateAsync(UpdateNhanVienDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");

            existing.Ten = dto.Ten;
            existing.Email = dto.Email;
            existing.SoDienThoai = dto.SoDienThoai;
            existing.DiaChi = dto.DiaChi;
            existing.NgayVaoLam = dto.NgayVaoLam;

            await _repo.UpdateAsync(existing);

            return new NhanVienDto
            {
                Id = existing.Id,
                Ten = existing.Ten,
                Email = existing.Email,
                SoDienThoai = existing.SoDienThoai,
                DiaChi = existing.DiaChi,
                NgayVaoLam = existing.NgayVaoLam
            };
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy nhân viên");
            await _repo.DeleteAsync(existing);
        }
    }
}

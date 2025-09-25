using Contracts;
using Entities.Models;
using Entities.DTOs;
using Service.Contracts;

namespace Service
{
    public class NgayLeService : INgayLeService
    {
        private readonly INgayLeRepository _repo;
        public NgayLeService(INgayLeRepository repo)
        {
            _repo = repo;
        }

        public async Task<NgayLeDto> CreateAsync(CreateNgayLeDto dto)
        {
            var entity = new NgayLe
            {
                TenNgayLe = dto.TenNgayLe,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc
            };

            var created = await _repo.CreateAsync(entity);

            return new NgayLeDto
            {
                Id = created.Id,
                TenNgayLe = created.TenNgayLe,
                NgayBatDau = created.NgayBatDau,
                NgayKetThuc = created.NgayKetThuc
            };
        }

        public async Task<NgayLeDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new NgayLeDto
            {
                Id = entity.Id,
                TenNgayLe = entity.TenNgayLe,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc
            };
        }

        public async Task<PagedResult<NgayLeDto>> GetPagedAsync(int page, int pageSize, string? ten = null)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ten);

            return new PagedResult<NgayLeDto>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalItems = paged.TotalItems,
                TotalPages = paged.TotalPages,
                Items = paged.Items.Select(n => new NgayLeDto
                {
                    Id = n.Id,
                    TenNgayLe = n.TenNgayLe,
                    NgayBatDau = n.NgayBatDau,
                    NgayKetThuc = n.NgayKetThuc
                })
            };
        }

        public async Task<NgayLeDto> UpdateAsync(UpdateNgayLeDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy ngày lễ.");

            entity.TenNgayLe = dto.TenNgayLe;
            entity.NgayBatDau = dto.NgayBatDau;
            entity.NgayKetThuc = dto.NgayKetThuc;

            await _repo.UpdateAsync(entity);

            return new NgayLeDto
            {
                Id = entity.Id,
                TenNgayLe = entity.TenNgayLe,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc
            };
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy ngày lễ.");

            await _repo.DeleteAsync(entity);
        }
    }
}

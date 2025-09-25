using Contracts;
using Entities.Models;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class NgayLeRepository : INgayLeRepository
    {
        private readonly RepositoryContext _context;
        public NgayLeRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<NgayLe> CreateAsync(NgayLe ngayLe)
        {
            // Kiểm tra trùng tên
            if (!string.IsNullOrWhiteSpace(ngayLe.TenNgayLe))
            {
                var exists = await _context.NgayLes
                    .AnyAsync(n => n.TenNgayLe.ToLower() == ngayLe.TenNgayLe.ToLower());

                if (exists)
                    throw new InvalidOperationException("Tên ngày lễ đã tồn tại.");
            }

            if (ngayLe.NgayKetThuc < ngayLe.NgayBatDau)
                throw new InvalidOperationException("Ngày kết thúc phải >= ngày bắt đầu.");

            _context.NgayLes.Add(ngayLe);
            await _context.SaveChangesAsync();
            return ngayLe;
        }

        public async Task<NgayLe?> GetByIdAsync(int id)
        {
            return await _context.NgayLes.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<PagedResult<NgayLe>> GetPagedAsync(int page, int pageSize, string? ten = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 8;

            var query = _context.NgayLes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(ten))
            {
                var keyword = ten.Trim().ToLower();
                query = query.Where(n => n.TenNgayLe.ToLower().Contains(keyword));
            }

            query = query.OrderBy(n => n.Id);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<NgayLe>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task UpdateAsync(NgayLe ngayLe)
        {
            // Kiểm tra trùng tên
            if (!string.IsNullOrWhiteSpace(ngayLe.TenNgayLe))
            {
                var exists = await _context.NgayLes
                    .AnyAsync(n => n.Id != ngayLe.Id && n.TenNgayLe.ToLower() == ngayLe.TenNgayLe.ToLower());

                if (exists)
                    throw new InvalidOperationException("Tên ngày lễ đã tồn tại.");
            }

            if (ngayLe.NgayKetThuc < ngayLe.NgayBatDau)
                throw new InvalidOperationException("Ngày kết thúc phải >= ngày bắt đầu.");

            _context.NgayLes.Update(ngayLe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(NgayLe ngayLe)
        {
            _context.NgayLes.Remove(ngayLe); // xóa cứng
            await _context.SaveChangesAsync();
        }
    }
}

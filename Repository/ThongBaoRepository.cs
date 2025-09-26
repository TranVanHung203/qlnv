using Contracts;
using Entities.Models;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly RepositoryContext _context;
        public ThongBaoRepository(RepositoryContext context) { _context = context; }

        public async Task<ThongBao> CreateAsync(ThongBao tb)
        {
            _context.ThongBaos.Add(tb);
            await _context.SaveChangesAsync();
            return tb;
        }

        public async Task<bool> ExistsForNhanVienOnDateAsync(int nhanVienId, DateTime dateUtc)
        {
            var d = dateUtc.Date;
            return await _context.ThongBaos.AnyAsync(t => t.NhanVienId == nhanVienId && t.NgayGui.Date == d);
        }

        public async Task<bool> ExistsForNhanVienWithReasonAsync(int nhanVienId, string reason, string emailNhan)
        {
            if (string.IsNullOrWhiteSpace(reason)) return false;
            if (string.IsNullOrWhiteSpace(emailNhan)) return false;
            var email = emailNhan.Trim().ToLower();
            return await _context.ThongBaos.AnyAsync(t => t.NhanVienId == nhanVienId && t.LyDo == reason && t.EmailNhan.ToLower() == email);
        }

        public async Task<PagedResult<ThongBao>> GetPagedAsync(int page, int pageSize, int? nhanVienId = null, string? emailNhan = null, DateTime? from = null, DateTime? to = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _context.ThongBaos.AsNoTracking().Include(t => t.NhanVien).AsQueryable();

            if (nhanVienId.HasValue) query = query.Where(t => t.NhanVienId == nhanVienId.Value);
            if (!string.IsNullOrWhiteSpace(emailNhan)) query = query.Where(t => t.EmailNhan.ToLower().Contains(emailNhan.Trim().ToLower()));
            if (from.HasValue) query = query.Where(t => t.NgayGui.Date >= from.Value.Date);
            if (to.HasValue) query = query.Where(t => t.NgayGui.Date <= to.Value.Date);

            query = query.OrderByDescending(t => t.NgayGui);

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<ThongBao>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }
    }
}
 

using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CauHinhThongBaoRepository : ICauHinhThongBaoRepository
    {
        private readonly RepositoryContext _context;
        public CauHinhThongBaoRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<CauHinhThongBao?> GetAsync()
        {
            // Prefer an explicitly active configuration. If none is active, fall back to the newest by Id.
            var active = await _context.CauHinhThongBaos
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (active != null) return active;

            return await _context.CauHinhThongBaos
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CauHinhThongBao>> GetAllAsync()
        {
            return await _context.CauHinhThongBaos.OrderByDescending(c => c.Id).ToListAsync();
        }

        public async Task<CauHinhThongBao?> GetByIdAsync(int id)
        {
            return await _context.CauHinhThongBaos.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task DeleteAsync(CauHinhThongBao entity)
        {
            _context.CauHinhThongBaos.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<CauHinhThongBao> CreateAsync(CauHinhThongBao cfg)
        {
            // Vô hiệu hóa tất cả cấu hình hiện tại
            var others = _context.CauHinhThongBaos.Where(c => c.IsActive);
            await others.ForEachAsync(c => c.IsActive = false);

            // Luôn set cấu hình mới là Active
            cfg.IsActive = true;

            _context.CauHinhThongBaos.Add(cfg);
            await _context.SaveChangesAsync();
            return cfg;
        }


        public async Task UpdateAsync(CauHinhThongBao cfg)
        {
            // If cfg is being marked active, ensure other actives are cleared
            if (cfg.IsActive)
            {
                var others = _context.CauHinhThongBaos.Where(c => c.IsActive && c.Id != cfg.Id);
                await others.ForEachAsync(c => c.IsActive = false);
            }

            _context.CauHinhThongBaos.Update(cfg);
            await _context.SaveChangesAsync();
        }
    }
}

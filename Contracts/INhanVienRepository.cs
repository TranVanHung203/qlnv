using Entities.Models;
using Entities.DTOs;

namespace Contracts
{
    public interface INhanVienRepository
    {
        Task<NhanVien> CreateAsync(NhanVien nhanVien);
        Task<NhanVien?> GetByIdAsync(int id);
        Task<List<NhanVien>> GetAllAsync();
        Task<PagedResult<NhanVien>> GetPagedAsync(int page, int pageSize, string? ten = null, string? sdt = null);
        Task UpdateAsync(NhanVien nhanVien);
        Task DeleteAsync(NhanVien nhanVien);
    }
}

using Entities.Models;
using Entities.DTOs;

namespace Contracts
{
    public interface INgayLeRepository
    {
        Task<NgayLe> CreateAsync(NgayLe ngayLe);
        Task<NgayLe?> GetByIdAsync(int id);
        Task<PagedResult<NgayLe>> GetPagedAsync(int page, int pageSize, string? ten = null);
        Task UpdateAsync(NgayLe ngayLe);
        Task DeleteAsync(NgayLe ngayLe);
    }
}

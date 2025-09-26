using Entities.Models;
using Entities.DTOs;

namespace Contracts
{
    public interface IThongBaoRepository
    {
        Task<ThongBao> CreateAsync(ThongBao tb);
        Task<bool> ExistsForNhanVienOnDateAsync(int nhanVienId, DateTime dateUtc);
    Task<bool> ExistsForNhanVienWithReasonAsync(int nhanVienId, string reason, string emailNhan);
        Task<PagedResult<ThongBao>> GetPagedAsync(int page, int pageSize, int? nhanVienId = null, string? emailNhan = null, DateTime? from = null, DateTime? to = null);
    }
}

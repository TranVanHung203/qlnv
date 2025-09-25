using Entities.DTOs;

namespace Service.Contracts
{
    public interface IThongBaoService
    {
        Task<PagedResult<ThongBaoDto>> GetPagedAsync(int page, int pageSize, int? nhanVienId = null, string? emailNhan = null, DateTime? from = null, DateTime? to = null);
    }
}

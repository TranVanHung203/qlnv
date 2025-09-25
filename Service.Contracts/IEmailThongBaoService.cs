using Entities.DTOs;

namespace Service.Contracts
{
    public interface IEmailThongBaoService
    {
        Task<EmailThongBaoDto> CreateAsync(CreateEmailThongBaoDto dto);
        Task<EmailThongBaoDto?> GetByIdAsync(int id);
        Task<PagedResult<EmailThongBaoDto>> GetPagedAsync(int page, int pageSize, string? email = null);
        Task<EmailThongBaoDto> UpdateAsync(UpdateEmailThongBaoDto dto);
        Task DeleteAsync(int id);
    }
}

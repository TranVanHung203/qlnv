using Entities.Models;
using Entities.DTOs;

namespace Contracts
{
    public interface IEmailThongBaoRepository
    {
        Task<EmailThongBao> CreateAsync(EmailThongBao email);
        Task<EmailThongBao?> GetByIdAsync(int id);
        Task<PagedResult<EmailThongBao>> GetPagedAsync(int page, int pageSize, string? email = null);
        Task UpdateAsync(EmailThongBao email);
        Task DeleteAsync(EmailThongBao email);
    }
}

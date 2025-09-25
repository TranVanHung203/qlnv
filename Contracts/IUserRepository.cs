using Entities.Models;
using Entities.DTOs;

namespace Contracts
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, string? q = null);
    }
}

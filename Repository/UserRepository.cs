using Contracts;
using Entities.Models;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RepositoryContext _context;
        public UserRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _context.Users.Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            user.IsDeleted = true; // soft delete
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, string? q = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Users.AsNoTracking().Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim().ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(keyword) || u.Email.ToLower().Contains(keyword));
            }

            var total = await query.CountAsync();
            var items = await query.OrderBy(u => u.Username)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<User>
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

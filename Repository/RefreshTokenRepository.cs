using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly RepositoryContext _context;
        public RefreshTokenRepository(RepositoryContext context) { _context = context; }

        public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(30)
            };

            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
            await _context.RefreshTokens.Include(r => r.User)
                                        .FirstOrDefaultAsync(r => r.Token == token);

        public async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken)
        {
            oldToken.IsRevoked = true;
            var newToken = new RefreshToken
            {
                UserId = oldToken.UserId,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(30)
            };
            await _context.RefreshTokens.AddAsync(newToken);
            await _context.SaveChangesAsync();
            return newToken;
        }
    }
}

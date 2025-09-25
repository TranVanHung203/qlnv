using Entities.Models;
using System;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateRefreshTokenAsync(Guid userId);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken);
    }
}

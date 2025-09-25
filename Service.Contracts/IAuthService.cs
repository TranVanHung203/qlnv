using Entities.DTOs;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task ForgotPasswordAsync(ForgotPasswordDto forgotDto);
        Task ResetPasswordAsync(ResetPasswordDto resetDto);
    }
}

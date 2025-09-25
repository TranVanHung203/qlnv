using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username bắt buộc")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password bắt buộc")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token bắt buộc")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới bắt buộc")]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token bắt buộc")]
        public string RefreshToken { get; set; }
    }
}

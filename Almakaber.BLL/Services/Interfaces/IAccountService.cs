using Almakaber.BLL.DTOs.Account;

namespace Almakaber.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> VerifyEmailOtpAsync(VerifyOtpDto dto);
        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<AuthResponseDto> ResetPasswordWithOtpAsync(ResetPasswordDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
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
        Task<AuthResponseDto> ResendOtpAsync(string email);
        Task<(IEnumerable<UserDto> Data, int TotalCount)> GetAllUsersAsync(int pageNumber, int pageSize, string? searchName = null, string? sortField = null, int sortOrder = 1);
        Task<AuthResponseDto> ToggleBlockUserAsync(string userId);
    }
}
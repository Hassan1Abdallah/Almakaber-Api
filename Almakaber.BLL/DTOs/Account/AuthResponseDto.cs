namespace Almakaber.BLL.DTOs.Account
{
    public class AuthResponseDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
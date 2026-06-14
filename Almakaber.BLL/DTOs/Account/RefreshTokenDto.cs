using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Account
{
    public class RefreshTokenDto
    {
        [Required] public string Token { get; set; }
        [Required] public string RefreshToken { get; set; }
    }
}
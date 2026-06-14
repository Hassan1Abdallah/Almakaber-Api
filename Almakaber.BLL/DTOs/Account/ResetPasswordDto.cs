using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string OtpCode { get; set; }
        [Required][MinLength(6)] public string NewPassword { get; set; }
    }
}
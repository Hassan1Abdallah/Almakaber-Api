using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Account
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress] 
        public string Email { get; set; }
        [Required] 
        public string OtpCode { get; set; }
    }
}
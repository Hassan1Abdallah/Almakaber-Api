using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress] 
        public string Email { get; set; }
    }
}
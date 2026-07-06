
using System.ComponentModel.DataAnnotations;


namespace Almakaber.BLL.DTOs.Account
{
    public class ResendOtpDto
    {
        [Required]
        [EmailAddress] 
        public string Email { get; set; }
    }
}

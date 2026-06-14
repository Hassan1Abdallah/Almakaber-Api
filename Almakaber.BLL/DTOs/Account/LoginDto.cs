using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
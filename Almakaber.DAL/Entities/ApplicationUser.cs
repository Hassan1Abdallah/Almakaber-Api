using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Almakaber.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "الاسم بالكامل مطلوب")]
        [MaxLength(100)]
        public string FullName { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<DeceasedSupplication> DeceasedSupplications { get; set; }

        public ApplicationUser()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
            DeceasedSupplications = new HashSet<DeceasedSupplication>(); 
        }
    }
}
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Deceased
{
    public class CreateDeceasedDto
    {
        [Required(ErrorMessage = "اسم المتوفي مطلوب")]
        [MaxLength(150)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "تاريخ الوفاة مطلوب")]
        public DateTime DateOfDeath { get; set; }

        public IFormFile Photo { get; set; }

        [Required(ErrorMessage = "يجب تحديد المقبرة")]
        public int GraveId { get; set; }
    }
}
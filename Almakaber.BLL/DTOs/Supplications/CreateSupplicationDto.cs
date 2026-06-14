using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Supplications
{
    public class CreateSupplicationDto
    {
        [Required(ErrorMessage = "عنوان الدعاء مطلوب")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "نص الدعاء مطلوب")]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
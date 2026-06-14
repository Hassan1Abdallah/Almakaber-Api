using System.ComponentModel.DataAnnotations;

namespace Almakaber.BLL.DTOs.Graves
{
    public class CreateGraveDto
    {
        [Required(ErrorMessage = "رقم الشارع مطلوب")]
        public int StreetNumber { get; set; }

        [Required(ErrorMessage = "رقم المقبرة مطلوب")]
        public int GraveNumber { get; set; }

        public string GenderType { get; set; }
    }
}
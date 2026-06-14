namespace Almakaber.BLL.DTOs.Deceased
{
    public class DeceasedDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfDeath { get; set; }
        public string ImageUrl { get; set; }

        // بيانات المقبرة عشان نعرضها في صفحة المتوفي مباشرة
        public int GraveId { get; set; }
        public int StreetNumber { get; set; }
        public int GraveNumber { get; set; }
    }
}
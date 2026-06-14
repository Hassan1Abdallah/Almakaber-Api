using System.ComponentModel.DataAnnotations;

namespace Almakaber.DAL.Entities
{
    public class Grave : BaseEntity
    {
        [Required]
        public int StreetNumber { get; set; }

        [Required]
        public int GraveNumber { get; set; }

        [MaxLength(20)]
        public string GenderType { get; set; }

        public virtual ICollection<Deceased> DeceasedPersons { get; set; }

        public Grave() : base()
        {
            DeceasedPersons = new HashSet<Deceased>();
        }

        // Helper Method للـ Update
        public void UpdateGrave(int streetNumber, int graveNumber, string genderType, string updatedByUserId)
        {
            StreetNumber = streetNumber;
            GraveNumber = graveNumber;
            GenderType = genderType;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedByUserId;
        }
    }
}
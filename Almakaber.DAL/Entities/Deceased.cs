using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Almakaber.DAL.Entities
{
    public class Deceased : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfDeath { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        public int GraveId { get; set; }
        [ForeignKey("GraveId")]
        public virtual Grave Grave { get; set; }

        public virtual ICollection<DeceasedSupplication> SupplicationCounters { get; set; }

        public Deceased() : base()
        {
            SupplicationCounters = new HashSet<DeceasedSupplication>();
        }

        public void UpdateDeceased(string fullName, DateTime dateOfDeath, int graveId, string imageUrl , string updatedByUserId)
        {
            FullName = fullName;
            DateOfDeath = dateOfDeath;
            ImageUrl = imageUrl;
            GraveId = graveId;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedByUserId;
        }
    }
}
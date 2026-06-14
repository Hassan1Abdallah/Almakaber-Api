using System.ComponentModel.DataAnnotations;

namespace Almakaber.DAL.Entities
{
    public class Supplication : BaseEntity
    {
        public string? Title { get; set; }
        [Required]
        
        public string Content { get; set; }

        public virtual ICollection<DeceasedSupplication> DeceasedSupplications { get; set; }

        public Supplication() : base()
        {
            DeceasedSupplications = new HashSet<DeceasedSupplication>();
        }
    }
}
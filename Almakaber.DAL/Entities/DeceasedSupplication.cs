using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Almakaber.DAL.Entities
{
    public class DeceasedSupplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Counter { get; set; }

        [Required]
        public int DeceasedId { get; set; }
        [ForeignKey("DeceasedId")]
        public virtual Deceased Deceased { get; set; }

        public bool WantsAnnualReminder { get; set; } = false;

        [Required]
        public int SupplicationId { get; set; }
        [ForeignKey("SupplicationId")]
        public virtual Supplication Supplication { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DeceasedSupplication()
        {
            Counter = 1; 
        }

        public void IncrementCounter()
        {
            Counter++;
        }
    }
}
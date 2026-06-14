using System.ComponentModel.DataAnnotations;

namespace Almakaber.DAL.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedById { get; set; } 

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedById { get; set; }

        public DateTime? DeletedAt { get; set; }
        public string? DeletedById { get; set; }

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        
        public void SoftDelete(string deletedByUserId)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedById = deletedByUserId;
        }
    }
}
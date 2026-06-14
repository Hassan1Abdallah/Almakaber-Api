using Almakaber.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Almakaber.DAL.Context
{
    public class AlmakaberDbContext : IdentityDbContext<ApplicationUser>
    {
        public AlmakaberDbContext(DbContextOptions<AlmakaberDbContext> options) : base(options)
        {
        }

        public DbSet<Grave> Graves { get; set; }
        public DbSet<Deceased> DeceasedPersons { get; set; }
        public DbSet<Supplication> Supplications { get; set; }
        public DbSet<DeceasedSupplication> DeceasedSupplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Grave>().HasQueryFilter(g => !g.IsDeleted);
            modelBuilder.Entity<Deceased>().HasQueryFilter(d => !d.IsDeleted);
            modelBuilder.Entity<Supplication>().HasQueryFilter(s => !s.IsDeleted);

            modelBuilder.Entity<Grave>()
                .HasMany(g => g.DeceasedPersons)
                .WithOne(d => d.Grave)
                .HasForeignKey(d => d.GraveId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; 
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
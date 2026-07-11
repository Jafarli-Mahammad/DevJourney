using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Company;
using Domain.Models.Entities.Partner;
using Domain.Models.Entities.University;
using Domain.Models.Entities.Student;
using Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DataAccessLayer.DataContexts
{
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
        public DbSet<UniversityProfile> UniversityProfiles { get; set; }
        public DbSet<PartnerProfile> PartnerProfiles { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Language> Languages { get; set; }


        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Identity");
            base.OnModelCreating(modelBuilder);  // this line
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var changes = ChangeTracker.Entries<IAuditableEntity>();

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userId, out Guid parsedUserId);


            if (changes != null)
            {
                foreach (var entry in changes.Where(m => m.State == EntityState.Added
                || m.State == EntityState.Modified
                || m.State == EntityState.Deleted))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedBy = parsedUserId;
                            entry.Entity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            entry.Property(m => m.CreatedBy).IsModified = false;
                            entry.Property(m => m.CreatedAt).IsModified = false;
                            entry.Entity.LastModifiedBy = parsedUserId;
                            entry.Entity.LastModifiedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            entry.Property(m => m.CreatedBy).IsModified = false;
                            entry.Property(m => m.CreatedAt).IsModified = false;
                            entry.Property(m => m.LastModifiedBy).IsModified = false;
                            entry.Property(m => m.LastModifiedAt).IsModified = false;
                            entry.Entity.DeletedBy = parsedUserId;
                            entry.Entity.DeletedAt = DateTime.UtcNow;
                            break;
                        default:
                            break;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

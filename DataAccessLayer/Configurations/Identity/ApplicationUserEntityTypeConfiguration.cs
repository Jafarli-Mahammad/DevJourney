using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Identity
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(m => m.FirstName).HasMaxLength(250).IsRequired();
            builder.Property(m => m.LastName).HasMaxLength(250).IsRequired();
            builder.ConfigureAuditable();

            builder.HasKey(m => m.Id);
            builder.ToTable("ApplicationUsers", "Identity");
        }
    }
}

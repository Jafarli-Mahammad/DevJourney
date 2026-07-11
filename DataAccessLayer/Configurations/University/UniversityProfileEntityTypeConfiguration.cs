using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.University;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.University
{
    public class UniversityProfileEntityTypeConfiguration : IEntityTypeConfiguration<UniversityProfile>
    {
        public void Configure(EntityTypeBuilder<UniversityProfile> builder)
        {
            builder.Property(x => x.UniversityName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.WebsiteUrl).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.Location).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.RepresentativeName).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.RepresentativeEmail).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.ApplicationUserId).IsRequired();
            builder.Property(x => x.IsVerified).IsRequired();
            builder.ConfigureAuditable();

            builder.HasKey(x => x.Id);
            builder.ToTable("UniversityProfiles", "University");

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<UniversityProfile>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
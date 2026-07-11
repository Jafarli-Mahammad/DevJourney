using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Company
{
    public class CompanyProfileEntityTypeConfiguration : IEntityTypeConfiguration<CompanyProfile>
    {
        public void Configure(EntityTypeBuilder<CompanyProfile> builder)
        {
            builder.Property(x => x.CompanyName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.CompanySector).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.WebsiteUrl).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.LinkedInUrl).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.Location).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.CompanySize).HasConversion<string>().IsRequired();
            builder.Property(x => x.ApplicationUserId).IsRequired();
            builder.Property(x => x.IsVerified).IsRequired();
            builder.ConfigureAuditable();

            builder.HasKey(x => x.Id);
            builder.ToTable("CompanyProfiles", "Company");

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<CompanyProfile>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
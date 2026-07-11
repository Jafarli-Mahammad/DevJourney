using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Partner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Partner
{
    public class PartnerProfileEntityTypeConfiguration : IEntityTypeConfiguration<PartnerProfile>
    {
        public void Configure(EntityTypeBuilder<PartnerProfile> builder)
        {
            builder.Property(x => x.PartnerName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.PartnerType).HasConversion<string>().IsRequired();
            builder.Property(x => x.WebsiteUrl).HasMaxLength(300).IsRequired(false);
            builder.Property(x => x.Location).HasMaxLength(150).IsRequired(false);
            builder.Property(x => x.Description).HasMaxLength(1000).IsRequired(false);
            builder.Property(x => x.ApplicationUserId).IsRequired();
            builder.Property(x => x.IsVerified).IsRequired();
            builder.ConfigureAuditable();

            builder.HasKey(x => x.Id);
            builder.ToTable("PartnerProfiles", "Partner");

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<PartnerProfile>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
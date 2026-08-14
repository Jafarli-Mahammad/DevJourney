using Domain.Models.Entities.Competition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.CompetitionConfig
{
    public class CompetitionConfiguration : IEntityTypeConfiguration<Domain.Models.Entities.Competition.Competition>
    {
        public void Configure(EntityTypeBuilder<Domain.Models.Entities.Competition.Competition> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title).IsRequired().HasMaxLength(255);
            builder.Property(c => c.ShortSummary).HasMaxLength(500);

            builder.HasOne(c => c.Partner)
                .WithMany()
                .HasForeignKey(c => c.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Stages)
                .WithOne(s => s.Competition)
                .HasForeignKey(s => s.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

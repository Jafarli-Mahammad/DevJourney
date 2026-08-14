using Domain.Models.Entities.Competition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.CompetitionConfig
{
    public class CompetitionStageConfiguration : IEntityTypeConfiguration<CompetitionStage>
    {
        public void Configure(EntityTypeBuilder<CompetitionStage> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).IsRequired().HasMaxLength(255);
        }
    }
}

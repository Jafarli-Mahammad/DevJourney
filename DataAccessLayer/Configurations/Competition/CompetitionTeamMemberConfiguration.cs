using Domain.Models.Entities.Competition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Competition;

public class CompetitionTeamMemberConfiguration : IEntityTypeConfiguration<CompetitionTeamMember>
{
    public void Configure(EntityTypeBuilder<CompetitionTeamMember> builder)
    {
        builder.HasKey(ctm => ctm.Id);

        builder.Property(ctm => ctm.Role)
            .HasMaxLength(100);
    }
}

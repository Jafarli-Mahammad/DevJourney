using Domain.Models.Entities.Competition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Competition;

public class CompetitionParticipantConfiguration : IEntityTypeConfiguration<CompetitionParticipant>
{
    public void Configure(EntityTypeBuilder<CompetitionParticipant> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(cp => cp.Members)
            .WithOne(m => m.Participant)
            .HasForeignKey(m => m.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cp => cp.Evaluations)
            .WithOne(e => e.Participant)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

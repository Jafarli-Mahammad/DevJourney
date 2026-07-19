using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class CorporateEventPostEntityTypeConfiguration : IEntityTypeConfiguration<CorporateEventPost>
    {
        public void Configure(EntityTypeBuilder<CorporateEventPost> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(80).IsRequired();
            builder.Property(x => x.EventTypeId).IsRequired();
            builder.Property(x => x.SpecialOccasion).HasMaxLength(100);
            builder.Property(x => x.StartAt).IsRequired();
            builder.Property(x => x.EndAt).IsRequired();
            builder.Property(x => x.Location).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Latitude).HasColumnType("decimal(18, 9)");
            builder.Property(x => x.Longitude).HasColumnType("decimal(18, 9)");
            builder.Property(x => x.TargetAudience).HasMaxLength(200).IsRequired();
            builder.Property(x => x.MaxAttendees).IsRequired();
            builder.Property(x => x.ConfidentialityNote).HasMaxLength(500);
            builder.Property(x => x.ApplicationMethod).HasConversion<string>().IsRequired();
            builder.Property(x => x.ApplicationLink).HasMaxLength(500);
            builder.Property(x => x.EventLanguage).HasMaxLength(50);
            builder.Property(x => x.IsPaid).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18, 2)");
            builder.ToTable("CorporateEventPosts", "Posts");

            builder.HasIndex(x => x.EventTypeId);

            builder.HasOne(x => x.EventType)
                .WithMany()
                .HasForeignKey(x => x.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.Agenda, sb =>
            {
                sb.ToTable("CorporateEventAgendaItems", "Posts");
                sb.Property(a => a.Time).HasMaxLength(50).IsRequired();
                sb.Property(a => a.Activity).HasMaxLength(255).IsRequired();
                sb.WithOwner().HasForeignKey(a => a.PostId);
                sb.HasKey(a => new { a.PostId, a.Order });
            });
        }
    }
}

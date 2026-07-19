using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class NetworkingEventPostEntityTypeConfiguration : IEntityTypeConfiguration<NetworkingEventPost>
    {
        public void Configure(EntityTypeBuilder<NetworkingEventPost> builder)
        {
            builder.Property(x => x.OrganizationName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Location).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Latitude).HasColumnType("decimal(18, 9)");
            builder.Property(x => x.Longitude).HasColumnType("decimal(18, 9)");
            builder.Property(x => x.StartAt).IsRequired();
            builder.Property(x => x.EndAt).IsRequired();
            builder.Property(x => x.MaxAttendees).IsRequired();
            builder.Property(x => x.TicketContact).HasMaxLength(100);
            builder.Property(x => x.IsPaid).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18, 2)");
            builder.ToTable("NetworkingEventPosts", "Posts");

            builder.OwnsMany(x => x.Stops, sb =>
            {
                sb.ToTable("NetworkingEventStops", "Posts");
                sb.Property(s => s.Description).HasMaxLength(500).IsRequired();
                sb.WithOwner().HasForeignKey(s => s.PostId);
                sb.HasKey(s => new { s.PostId, s.Order });
            });
        }
    }
}

using Domain.Models.Entities;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class TeamSearchPostIdeaFieldEntityTypeConfiguration : IEntityTypeConfiguration<TeamSearchPostIdeaField>
    {
        public void Configure(EntityTypeBuilder<TeamSearchPostIdeaField> builder)
        {
            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.IdeaFieldId).IsRequired();
            builder.HasKey(x => new { x.PostId, x.IdeaFieldId });
            builder.ToTable("TeamSearchPostIdeaFields", "Posts");

            builder.HasOne(x => x.Post)
                .WithMany(x => x.IdeaFields)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.IdeaField)
                .WithMany()
                .HasForeignKey(x => x.IdeaFieldId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
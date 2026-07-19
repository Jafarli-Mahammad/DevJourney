using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class TeamSearchPostEntityTypeConfiguration : IEntityTypeConfiguration<TeamSearchPost>
    {
        public void Configure(EntityTypeBuilder<TeamSearchPost> builder)
        {
            //builder.HasKey(x => x.Id);
            builder.Property(x => x.Note).HasMaxLength(150);
            builder.ToTable("TeamSearchPosts", "Posts");

            builder.HasMany(x => x.IdeaFields)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
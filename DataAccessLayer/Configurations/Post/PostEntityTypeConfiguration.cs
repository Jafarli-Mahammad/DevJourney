using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostEntity = Domain.Models.Entities.Post.Post;

namespace DataAccessLayer.Configurations.Post
{
    public class PostEntityTypeConfiguration : IEntityTypeConfiguration<PostEntity>
    {
        public void Configure(EntityTypeBuilder<PostEntity> builder)
        {
            builder.ConfigureAuditable();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AuthorId).IsRequired();
            builder.Property(x => x.Type).HasConversion<string>().IsRequired();
            builder.Property(x => x.IsEdited).IsRequired();
            builder.ToTable("Posts", "Posts");
            builder.HasIndex(x => x.CreatedAt).IsDescending();
            builder.HasQueryFilter(x => x.DeletedAt == null);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using Domain.Models.Entities;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class TeamMemberSearchPostRoleEntityTypeConfiguration : IEntityTypeConfiguration<TeamMemberSearchPostRole>
    {
        public void Configure(EntityTypeBuilder<TeamMemberSearchPostRole> builder)
        {
            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.RoleId).IsRequired();
            builder.HasKey(x => new { x.PostId, x.RoleId });
            builder.ToTable("TeamMemberSearchPostRoles", "Posts");

            builder.HasOne(x => x.Post)
                .WithMany(x => x.TargetRoles)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
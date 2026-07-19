using Domain.Models.Entities.Post;
using Domain.Models.Entities;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class TeamMemberSearchPostEntityTypeConfiguration : IEntityTypeConfiguration<TeamMemberSearchPost>
    {
        public void Configure(EntityTypeBuilder<TeamMemberSearchPost> builder)
        {
            //builder.HasKey(x => x.Id);
            builder.Property(x => x.IdeaFieldId).IsRequired();
            builder.Property(x => x.TeamName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.MembersNeeded).IsRequired();
            builder.Property(x => x.WorkMode).HasConversion<string>().IsRequired();
            builder.Property(x => x.SocialLink).IsRequired();
            builder.Property(x => x.LookingForLocation).HasMaxLength(100);
            builder.Property(x => x.AdditionalNote).HasMaxLength(100);
            builder.ToTable("TeamMemberSearchPosts", "Posts");

            builder.HasOne(x => x.IdeaField)
                .WithMany()
                .HasForeignKey(x => x.IdeaFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LookingForLanguage)
                .WithMany()
                .HasForeignKey(x => x.LookingForLanguageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TargetRoles)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.TargetSkills)
                .WithOne()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
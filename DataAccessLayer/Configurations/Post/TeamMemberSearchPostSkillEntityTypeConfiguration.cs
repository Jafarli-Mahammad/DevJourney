using Domain.Models.Entities.Post;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class TeamMemberSearchPostSkillEntityTypeConfiguration : IEntityTypeConfiguration<TeamMemberSearchPostSkill>
    {
        public void Configure(EntityTypeBuilder<TeamMemberSearchPostSkill> builder)
        {
            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.SkillId).IsRequired();
            builder.HasKey(x => new { x.PostId, x.SkillId });
            builder.ToTable("TeamMemberSearchPostSkills", "Posts");

            builder.HasOne<TeamMemberSearchPost>()
                .WithMany(x => x.TargetSkills)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
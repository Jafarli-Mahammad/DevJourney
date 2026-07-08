using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations
{
    public class StudentSkillEntityTypeConfiguration : IEntityTypeConfiguration<StudentSkill>
    {
        public void Configure(EntityTypeBuilder<StudentSkill> builder)
        {
            builder.Property(s => s.StudentProfileId).IsRequired();
            builder.Property(s => s.SkillId).IsRequired();

            builder.HasKey(s => new { s.StudentProfileId, s.SkillId });
            builder.ToTable("StudentSkills", "Student");

            builder.HasOne(ss => ss.StudentProfile)
                .WithMany(sp => sp.StudentSkills)
                .HasForeignKey(ss => ss.StudentProfileId);

            builder.HasOne(ss => ss.Skill)
                .WithMany(s => s.StudentSkills)
                .HasForeignKey(ss => ss.SkillId);
        }
    }
}
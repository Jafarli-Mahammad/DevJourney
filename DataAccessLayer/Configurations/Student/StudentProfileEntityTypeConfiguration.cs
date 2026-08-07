using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Student;
using Domain.Models.Entities.University;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class StudentProfileEntityTypeConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            builder.Property(m => m.FirstName).HasMaxLength(250).IsRequired();
            builder.Property(m => m.LastName).HasMaxLength(250).IsRequired();
            builder.Property(m => m.UniversityId).IsRequired(false);

            builder.Property(s => s.PhoneNumber).HasMaxLength(50).IsRequired(false);
            builder.Property(s => s.ProfessionId).IsRequired(false);
            builder.Property(s => s.Course).HasMaxLength(50).IsRequired(false);

            builder.Property(s => s.GitHubUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.LinkedinUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.PortfolioUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.CVUrl).HasColumnType("varchar(200)").IsRequired(false);

            builder.Property(s => s.MainRoleId).IsRequired(false);
            builder.Property(s => s.ExperienceLevel).HasConversion<string>().IsRequired(false);

            builder.Property(s => s.Bio).HasMaxLength(500).IsRequired(false);
            builder.Property(s => s.ApplicationUserId).IsRequired();

            builder.ConfigureAuditable();
            builder.HasKey(s => s.Id);
            builder.ToTable("StudentProfiles", "Student");

            builder.Navigation(s => s.StudentSkills).HasField("_studentSkills");
            builder.Navigation(s => s.StudentLanguages).HasField("_studentLanguages");

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<StudentProfile>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.University)
                .WithMany()
                .HasForeignKey(s => s.UniversityId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Profession)
                .WithMany()
                .HasForeignKey(s => s.ProfessionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.MainRole)
                .WithMany()
                .HasForeignKey(s => s.MainRoleId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

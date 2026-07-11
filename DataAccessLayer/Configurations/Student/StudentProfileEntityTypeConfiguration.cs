using DataAccessLayer.Configurations.Helper;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Student;
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
            builder.Property(s => s.Location).HasMaxLength(100).IsRequired(false);
            //builder.Property(m => m.Role).HasConversion<string>().IsRequired();
            builder.Property(s => s.CVUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.LinkedinUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.GitHubUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.Experience).HasConversion<string>().IsRequired();
            builder.Property(s => s.Achievements).HasMaxLength(250).IsRequired();
            builder.Property(s => s.Bio).HasMaxLength(500).IsRequired(false);
            builder.Property(s => s.PreferredWorkFormat).HasConversion<string>().IsRequired();
            builder.Property(s => s.ApplicationUserId).IsRequired();
            builder.ConfigureAuditable();

            builder.Navigation(s => s.StudentSkills).HasField("_studentSkills");
            builder.Navigation(s => s.StudentLanguages).HasField("_studentLanguages");

            builder.HasKey(s => s.Id);
            builder.ToTable("StudentProfiles", "Student");

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<StudentProfile>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
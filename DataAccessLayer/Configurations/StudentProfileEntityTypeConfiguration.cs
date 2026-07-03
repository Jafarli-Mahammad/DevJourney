using DataAccessLayer.Configurations.Helper;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations
{
    public class StudentProfileEntityTypeConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            builder.Property(s => s.Location).HasMaxLength(100).IsRequired(false);
            builder.Property(m => m.Role).HasConversion<string>().IsRequired();
            builder.Property(s => s.CVUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.LinkedinUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.GitHubUrl).HasColumnType("varchar(200)").IsRequired(false);
            builder.Property(s => s.Experience).HasConversion<string>().IsRequired();
            builder.Property(s => s.Achievements).HasMaxLength(250).IsRequired();
            builder.Property(s => s.Bio).HasMaxLength(500).IsRequired(false);
            builder.Property(s => s.PreferredWorkFormat).HasConversion<string>().IsRequired();
            builder.Property(s => s.ApplicationUserId).IsRequired();
            builder.ConfigureAuditable();

            builder.HasKey(s => s.Id);
            builder.ToTable("StudentProfiles", "Student");
        }
    }
}

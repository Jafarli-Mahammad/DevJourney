using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class StudentLanguageEntityTypeConfiguration : IEntityTypeConfiguration<StudentLanguage>
    {
        public void Configure(EntityTypeBuilder<StudentLanguage> builder)
        {
            builder.Property(s => s.ProficiencyLevel).HasConversion<string>();
            builder.Property(s => s.StudentProfileId).IsRequired();
            builder.Property(s => s.LanguageId).IsRequired();

            builder.HasKey(s => new { s.StudentProfileId, s.LanguageId });
            builder.ToTable("StudentLanguages", "Student");

            builder.HasOne(ss => ss.StudentProfile)
                .WithMany(sp => sp.StudentLanguages)
                .HasForeignKey(ss => ss.StudentProfileId);

            builder.HasOne(ss => ss.Language)
                .WithMany(s => s.StudentLanguages)
                .HasForeignKey(ss => ss.LanguageId);
        }
    }
}
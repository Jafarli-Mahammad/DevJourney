using DataAccessLayer.Configurations.Helper;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.ConfigureAuditable();

            builder.HasKey(s => s.Id);
            builder.ToTable("Languages", "Student");
        }
    }
}
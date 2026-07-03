using DataAccessLayer.Configurations.Helper;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations
{
    public class SkillEntityTypeConfiguration : IEntityTypeConfiguration<Langauge>
    {
        public void Configure(EntityTypeBuilder<Langauge> builder)
        {
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.ConfigureAuditable();

            builder.HasKey(s => s.Id);
            builder.ToTable("Skills", "Student");
        }
    }
}
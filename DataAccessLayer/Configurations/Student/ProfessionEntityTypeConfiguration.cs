using DataAccessLayer.Configurations.Helper;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class ProfessionEntityTypeConfiguration : IEntityTypeConfiguration<Profession>
    {
        public void Configure(EntityTypeBuilder<Profession> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(250).IsRequired();
            builder.ConfigureAuditable();
            builder.HasKey(p => p.Id);
            builder.ToTable("Professions", "Student");
        }
    }
}

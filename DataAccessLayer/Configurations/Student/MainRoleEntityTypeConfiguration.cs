using DataAccessLayer.Configurations.Helper;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Student
{
    public class MainRoleEntityTypeConfiguration : IEntityTypeConfiguration<MainRole>
    {
        public void Configure(EntityTypeBuilder<MainRole> builder)
        {
            builder.Property(r => r.Name).HasMaxLength(250).IsRequired();
            builder.ConfigureAuditable();
            builder.HasKey(r => r.Id);
            builder.ToTable("MainRoles", "Student");
        }
    }
}

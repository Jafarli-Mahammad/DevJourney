 using DataAccessLayer.Configurations.Helper;
using Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
            builder.ConfigureAuditable();

            builder.HasKey(r => r.Id);
            builder.ToTable("Role", "Identity");
        }
    }
}

using Domain.Models.Entities.Identity;
using Domain.Models.Entities.Student;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Identity
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(m => m.FirstName).HasColumnType("nvarchar").HasMaxLength(250).IsRequired();
            builder.Property(m => m.LastName).HasColumnType("nvarchar").HasMaxLength(250).IsRequired();
            builder.Property(m => m.CreatedAt).HasColumnType("datetime2").IsRequired();
            builder.Property(m => m.LastModifiedAt).HasColumnType("datetime2");
            builder.Property(m => m.DeletedAt).HasColumnType("datetime2");
            builder.Property(m => m.IsDeleted).IsRequired();

            builder.HasKey(m => m.Id);
            builder.ToTable("ApplicationUsers", "Identity");

            builder.HasOne<StudentProfile>()
                .WithOne(s => s.ApplicationUser)
                .HasForeignKey<StudentProfile>(s => s.ApplicationUserId);
        }
    }
}

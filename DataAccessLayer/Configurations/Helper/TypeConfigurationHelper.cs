using Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Helper
{
    static class TypeConfigurationHelper
    {
        public static EntityTypeBuilder<T> ConfigureAuditable<T>(this EntityTypeBuilder<T> builder)
            where T : class, IAuditableEntity
        {
            builder.Property(m => m.CreatedAt).HasColumnType("datetime2").IsRequired();
            builder.Property(m => m.CreatedBy);
            builder.Property(m => m.LastModifiedAt).HasColumnType("datetime2");
            builder.Property(m => m.LastModifiedBy);
            builder.Property(m => m.DeletedAt).HasColumnType("datetime2");
            builder.Property(m => m.DeletedBy);
            builder.Property(m => m.IsDeleted).IsRequired();
            return builder;
        }
    }
}

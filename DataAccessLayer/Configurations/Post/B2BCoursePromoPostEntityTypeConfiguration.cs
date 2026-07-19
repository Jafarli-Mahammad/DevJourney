using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations.Post
{
    public class B2BCoursePromoPostEntityTypeConfiguration : IEntityTypeConfiguration<B2BCoursePromoPost>
    {
        public void Configure(EntityTypeBuilder<B2BCoursePromoPost> builder)
        {
            builder.Property(x => x.CourseName).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(80).IsRequired();
            builder.Property(x => x.CourseTypeId).IsRequired();
            builder.Property(x => x.EventFormat).HasConversion<string>().IsRequired();
            builder.Property(x => x.TargetMajor).HasMaxLength(200).IsRequired();
            builder.Property(x => x.StartAt).IsRequired();
            builder.Property(x => x.EndAt).IsRequired();
            builder.Property(x => x.LocationOrLink).HasMaxLength(500).IsRequired();
            builder.Property(x => x.MaxAttendees).IsRequired();
            builder.Property(x => x.DurationInfo).HasMaxLength(100);
            builder.Property(x => x.IsPaid).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18, 2)");
            builder.Property(x => x.HasDiscount).IsRequired();
            builder.Property(x => x.DiscountNote).HasMaxLength(200);
            builder.Property(x => x.HasCertificate);
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.ApplicationMethod).HasConversion<string>().IsRequired();
            builder.Property(x => x.ApplicationLink).HasMaxLength(500);
            builder.ToTable("B2BCoursePromoPosts", "Posts");

            builder.HasIndex(x => x.CourseTypeId);

            builder.HasOne(x => x.CourseType)
                .WithMany()
                .HasForeignKey(x => x.CourseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Instructor, sb =>
            {
                sb.Property(i => i.Name).HasMaxLength(100).HasColumnName("InstructorName");
                sb.Property(i => i.LinkedIn).HasMaxLength(255).HasColumnName("InstructorLinkedIn");
                sb.Property(i => i.ReviewsLink).HasMaxLength(255).HasColumnName("InstructorReviewsLink");
            });
        }
    }
}

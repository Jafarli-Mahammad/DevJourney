using Domain.Models.Enums;

namespace Domain.Models.Entities.Post
{
    public class InstructorProfile
    {
        public string? Name { get; set; }
        public string? LinkedIn { get; set; }
        public string? ReviewsLink { get; set; }
    }

    public class B2BCoursePromoPost : Post
    {
        public string CourseName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public Guid CourseTypeId { get; set; }
        public CourseType CourseType { get; set; } = null!;
        public EventFormat EventFormat { get; set; }
        public string TargetMajor { get; set; } = null!;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string LocationOrLink { get; set; } = null!;
        public int MaxAttendees { get; set; }
        public string? DurationInfo { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public bool HasDiscount { get; set; }
        public string? DiscountNote { get; set; }
        public bool? HasCertificate { get; set; }
        public string Content { get; set; } = null!;
        public ApplicationMethod ApplicationMethod { get; set; }
        public string? ApplicationLink { get; set; }

        public InstructorProfile Instructor { get; set; } = null!;
    }
}

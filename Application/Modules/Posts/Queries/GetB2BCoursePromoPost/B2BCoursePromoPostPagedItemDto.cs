using System;
using Domain.Models.Enums;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public class B2BCoursePromoPostPagedItemDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Title { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public Guid CourseTypeId { get; set; }
        public string CourseTypeName { get; set; } = null!;
        public EventFormat EventFormat { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string LocationOrLink { get; set; } = null!;
        public int MaxAttendees { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public bool HasDiscount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

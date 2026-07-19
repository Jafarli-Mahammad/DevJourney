using System;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public class CorporateEventPostPagedItemDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Title { get; set; } = null!;
        public Guid EventTypeId { get; set; }
        public string EventTypeName { get; set; } = null!;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Location { get; set; } = null!;
        public string TargetAudience { get; set; } = null!;
        public int MaxAttendees { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

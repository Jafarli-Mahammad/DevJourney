using System;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public class NetworkingEventPostPagedItemDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string OrganizationName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int MaxAttendees { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

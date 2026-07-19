using Domain.Models.Enums;

namespace Domain.Models.Entities.Post
{
    public class NetworkingEventStop
    {
        public Guid PostId { get; set; }
        public int Order { get; set; }
        public string Description { get; set; } = null!;
    }

    public class NetworkingEventPost : Post
    {
        public string OrganizationName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int MaxAttendees { get; set; }
        public string? TicketContact { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }

        public ICollection<NetworkingEventStop> Stops { get; set; } = new List<NetworkingEventStop>();
    }
}

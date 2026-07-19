using Domain.Models.Enums;

namespace Domain.Models.Entities.Post
{
    public class CorporateEventAgendaItem
    {
        public Guid PostId { get; set; }
        public int Order { get; set; }
        public string Time { get; set; } = null!;
        public string Activity { get; set; } = null!;
    }

    public class CorporateEventPost : Post
    {
        public string Title { get; set; } = null!;
        public Guid EventTypeId { get; set; }
        public EventType EventType { get; set; } = null!;
        public string? SpecialOccasion { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Location { get; set; } = null!;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string TargetAudience { get; set; } = null!;
        public int MaxAttendees { get; set; }
        public string? ConfidentialityNote { get; set; }
        public ApplicationMethod ApplicationMethod { get; set; }
        public string? ApplicationLink { get; set; }
        public string? EventLanguage { get; set; }
        public bool IsPaid { get; set; }
        public decimal? Price { get; set; }

        public ICollection<CorporateEventAgendaItem> Agenda { get; set; } = new List<CorporateEventAgendaItem>();
    }
}

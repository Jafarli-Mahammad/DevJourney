using System;
using Domain.Models.Enums;

namespace Application.Modules.Competitions.Dtos
{
    public class AvailableCompetitionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortSummary { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public string? Location { get; set; }
        public string? CoverImageUrl { get; set; }
        public ParticipationFormat ParticipationFormat { get; set; }
        public int MaxTeamSize { get; set; }
        public string? PartnerName { get; set; }
    }
}

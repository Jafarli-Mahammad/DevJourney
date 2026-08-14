using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Competition;

public class Evaluation : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid JuryId { get; set; }
    public int InnovationScore { get; set; }
    public int TechnicalScore { get; set; }
    public int PitchScore { get; set; }
    public string? Feedback { get; set; }

    public virtual CompetitionParticipant Participant { get; set; } = null!;
}

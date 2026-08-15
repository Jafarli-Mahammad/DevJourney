using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Competition;

public class Evaluation : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid JuryId { get; set; }
    public Guid CriterionId { get; set; }
    public int Score { get; set; }
    public string? Comments { get; set; }

    public virtual CompetitionParticipant Participant { get; set; } = null!;
}

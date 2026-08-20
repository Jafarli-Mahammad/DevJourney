using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Competition;

public class CompetitionTeamMember : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid StudentProfileId { get; set; }
    public string? Role { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckInTime { get; set; }

    public Guid? TeamId { get; set; }
    public virtual Team? Team { get; set; }

    public virtual CompetitionParticipant Participant { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using Domain.Models.Abstracts;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Competition;

public class CompetitionParticipant : BaseEntity
{
    public Guid CompetitionId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsTeam { get; set; }
    public Guid? CaptainId { get; set; }
    public Guid? IndividualStudentId { get; set; }
    public DateTime AppliedAt { get; set; }
    public ApplicationStatus Status { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectDescription { get; set; }
    public string? GithubUrl { get; set; }
    public string? PitchDeckAssetId { get; set; }
    public DateTime? HoldAt { get; set; }
    public bool IsFinalist { get; set; }

    public virtual ICollection<CompetitionTeamMember> Members { get; set; } = new List<CompetitionTeamMember>();
    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
}

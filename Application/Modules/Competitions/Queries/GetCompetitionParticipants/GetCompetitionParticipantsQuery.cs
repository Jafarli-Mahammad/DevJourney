using System;
using System.Collections.Generic;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Competitions.Queries.GetCompetitionParticipants;

public class GetCompetitionParticipantsQuery : IRequest<List<CompetitionParticipantDto>>
{
    public Guid CompetitionId { get; set; }
    public ApplicationStatus? Status { get; set; }
}

public class CompetitionParticipantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsTeam { get; set; }
    public ApplicationStatus Status { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime AppliedAt { get; set; }
    public string? ProjectName { get; set; }
}

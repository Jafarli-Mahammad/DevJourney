using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Modules.Competitions.Queries.GetScoreboard;

public class GetScoreboardQuery : IRequest<List<ScoreboardDto>>
{
    public Guid CompetitionId { get; set; }
}

public class ScoreboardDto
{
    public Guid ParticipantId { get; set; }
    public string Name { get; set; } = null!;
    public int TotalScore { get; set; }
    public int Rank { get; set; }
}

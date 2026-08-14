using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Modules.Competitions.Queries.GetCompetitionStages;

public class GetCompetitionStagesQuery : IRequest<List<CompetitionStageDto>>
{
    public Guid CompetitionId { get; set; }
}

public class CompetitionStageDto
{
    public Guid Id { get; set; }
    public int DayNumber { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

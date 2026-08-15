using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories.Competitions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetCompetitionStages;

public class GetCompetitionStagesHandler : IRequestHandler<GetCompetitionStagesQuery, List<CompetitionStageDto>>
{
    private readonly ICompetitionRepository _repository;

    public GetCompetitionStagesHandler(ICompetitionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CompetitionStageDto>> Handle(GetCompetitionStagesQuery request, CancellationToken cancellationToken)
    {
        var competition = await _repository.GetAsync(
            c => c.Id == request.CompetitionId,
            q => q.Include(c => c.Stages),
            cancellationToken);

        if (competition == null)
            throw new Application.Exceptions.NotFoundException("Competition", request.CompetitionId);

        return competition.Stages.Select(s => new CompetitionStageDto
        {
            Id = s.Id,
            DayNumber = s.DayNumber,
            Title = s.Title,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();
    }
}

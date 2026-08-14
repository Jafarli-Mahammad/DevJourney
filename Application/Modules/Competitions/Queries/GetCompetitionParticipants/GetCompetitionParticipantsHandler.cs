using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories.Competitions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetCompetitionParticipants;

public class GetCompetitionParticipantsHandler : IRequestHandler<GetCompetitionParticipantsQuery, List<CompetitionParticipantDto>>
{
    private readonly ICompetitionParticipantRepository _repository;

    public GetCompetitionParticipantsHandler(ICompetitionParticipantRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CompetitionParticipantDto>> Handle(GetCompetitionParticipantsQuery request, CancellationToken cancellationToken)
    {
        var participants = await _repository.GetAllAsync(p => p.CompetitionId == request.CompetitionId, cancellationToken);

        if (request.Status.HasValue)
        {
            participants = participants.Where(p => p.Status == request.Status.Value).ToList();
        }

        return participants.Select(p => new CompetitionParticipantDto
        {
            Id = p.Id,
            Name = p.Name,
            IsTeam = p.IsTeam,
            Status = p.Status,
            IsCheckedIn = p.IsCheckedIn,
            AppliedAt = p.AppliedAt,
            ProjectName = p.ProjectName
        }).ToList();
    }
}

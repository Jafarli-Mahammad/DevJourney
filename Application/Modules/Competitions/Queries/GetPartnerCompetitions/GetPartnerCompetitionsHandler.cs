using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories.Competitions;
using Domain.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetPartnerCompetitions;

public class GetPartnerCompetitionsHandler : IRequestHandler<GetPartnerCompetitionsQuery, List<PartnerCompetitionDto>>
{
    private readonly ICompetitionRepository _competitionRepository;
    private readonly ICompetitionParticipantRepository _participantRepository;

    public GetPartnerCompetitionsHandler(ICompetitionRepository competitionRepository, ICompetitionParticipantRepository participantRepository)
    {
        _competitionRepository = competitionRepository;
        _participantRepository = participantRepository;
    }

    public async Task<List<PartnerCompetitionDto>> Handle(GetPartnerCompetitionsQuery request, CancellationToken cancellationToken)
    {
        var competitions = await _competitionRepository.GetAllAsync(c => c.PartnerId == request.PartnerId, cancellationToken);

        var result = new List<PartnerCompetitionDto>();

        foreach (var competition in competitions)
        {
            var participants = await _participantRepository.GetAllAsync(p => p.CompetitionId == competition.Id, cancellationToken);

            result.Add(new PartnerCompetitionDto
            {
                Id = competition.Id,
                Title = competition.Title,
                ApplicantCount = participants.Count,
                ApprovedCount = participants.Count(p => p.Status == ApplicationStatus.Approved),
                CheckInCount = participants.Count(p => p.IsCheckedIn),
                TeamCount = participants.Count(p => p.IsTeam)
            });
        }

        return result;
    }
}

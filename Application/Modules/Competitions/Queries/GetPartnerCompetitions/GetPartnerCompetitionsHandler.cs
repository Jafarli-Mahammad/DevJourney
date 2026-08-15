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
        if (competitions.Count == 0)
        {
            return new List<PartnerCompetitionDto>();
        }

        var competitionIds = competitions.Select(c => c.Id).ToList();
        var allParticipants = await _participantRepository.GetAllAsync(p => competitionIds.Contains(p.CompetitionId), cancellationToken);
        var participantsByCompetition = allParticipants
            .GroupBy(p => p.CompetitionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<PartnerCompetitionDto>();

        foreach (var competition in competitions)
        {
            participantsByCompetition.TryGetValue(competition.Id, out var participants);
            var participantList = participants ?? [];

            result.Add(new PartnerCompetitionDto
            {
                Id = competition.Id,
                Title = competition.Title,
                ApplicantCount = participantList.Count,
                ApprovedCount = participantList.Count(p => p.Status == ApplicationStatus.Approved),
                CheckInCount = participantList.Count(p => p.IsCheckedIn),
                TeamCount = participantList.Count(p => p.IsTeam)
            });
        }

        return result;
    }
}

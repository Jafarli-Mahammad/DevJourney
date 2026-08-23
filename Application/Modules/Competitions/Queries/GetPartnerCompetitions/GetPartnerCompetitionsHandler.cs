using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Enums;
using MediatR;
using Application.Repositories.Competitions;
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
        var competitions = await _competitionRepository.GetAllAsync(
            c => c.PartnerId == request.PartnerId,
            q => q.Include(c => c.Stages),
            cancellationToken);

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
                ShortSummary = competition.ShortSummary,
                Description = competition.Description,
                StartDate = competition.StartDate,
                EndDate = competition.EndDate,
                RegistrationDeadline = competition.RegistrationDeadline,
                SubmissionDeadline = competition.SubmissionDeadline,
                Location = competition.Location,
                LocationMapLink = competition.LocationMapLink,
                Tags = competition.Tags,
                EvaluationCriteria = competition.EvaluationCriteria,
                CoverImageUrl = competition.CoverImageUrl,
                ContactEmail = competition.ContactEmail,
                ContactPhone = competition.ContactPhone,
                ContactSocialLink = competition.ContactSocialLink,
                ParticipationFormat = (int)competition.ParticipationFormat,
                MaxTeamSize = competition.MaxTeamSize,
                IsPublished = competition.IsPublished,
                IsRegistrationOpen = competition.IsRegistrationOpen,
                IsJuryActive = competition.IsJuryActive,
                IsScoreboardLive = competition.IsScoreboardLive,
                IsCertificatesPublished = competition.IsCertificatesPublished,
                AgendaMode = competition.AgendaMode ?? "MANUAL",
                AgendaPdfUrl = competition.AgendaPdfUrl,
                Stages = competition.Stages?.Select(s => new PartnerCompetitionStageDto
                {
                    Id = s.Id,
                    DayNumber = s.DayNumber,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).OrderBy(s => s.DayNumber).ThenBy(s => s.StartTime).ToList() ?? new List<PartnerCompetitionStageDto>(),
                ApplicantCount = participantList.Count,
                ApprovedCount = participantList.Count(p => p.Status == ApplicationStatus.Approved),
                CheckInCount = participantList.Count(p => p.IsCheckedIn),
                TeamCount = participantList.Count(p => p.IsTeam)
            });
        }

        return result;
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.Competitions.Dtos;
using Application.Repositories.Competitions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Application.Modules.Competitions.Queries.GetCompetitionDetails;

public class GetCompetitionDetailsQuery : IRequest<object?>
{
    public Guid Id { get; set; }
}

public class GetCompetitionDetailsQueryHandler : IRequestHandler<GetCompetitionDetailsQuery, object?>
{
    private readonly ICompetitionRepository _competitionRepository;
    private readonly HybridCache _hybridCache;

    public GetCompetitionDetailsQueryHandler(
        ICompetitionRepository competitionRepository,
        HybridCache hybridCache)
    {
        _competitionRepository = competitionRepository;
        _hybridCache = hybridCache;
    }

    public async Task<object?> Handle(GetCompetitionDetailsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"competitions:details:{request.Id}";
        var details = await _hybridCache.GetOrCreateAsync<CompetitionDetailsDto?>(
            cacheKey,
            async token =>
            {
                var c = await _competitionRepository.GetAsync(
                    x => x.Id == request.Id,
                    q => q.Include(x => x.Partner).Include(x => x.Stages),
                    token);

                if (c == null)
                    return null;

                return new CompetitionDetailsDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ShortSummary = c.ShortSummary,
                    Description = c.Description,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    RegistrationDeadline = c.RegistrationDeadline,
                    SubmissionDeadline = c.SubmissionDeadline,
                    Location = c.Location,
                    LocationMapLink = c.LocationMapLink,
                    CoverImageUrl = c.CoverImageUrl,
                    ParticipationFormat = c.ParticipationFormat,
                    MaxTeamSize = c.MaxTeamSize,
                    Tags = c.Tags,
                    EvaluationCriteria = c.EvaluationCriteria,
                    ContactEmail = c.ContactEmail,
                    ContactPhone = c.ContactPhone,
                    ContactSocialLink = c.ContactSocialLink,
                    GitHubRepositoryRequirement = c.GitHubRepositoryRequirement,
                    LiveDeploymentRequirement = c.LiveDeploymentRequirement,
                    PitchDeckFormat = c.PitchDeckFormat,
                    AgendaMode = c.AgendaMode,
                    AgendaPdfUrl = c.AgendaPdfUrl,
                    IsPublished = c.IsPublished,
                    IsRegistrationOpen = c.IsRegistrationOpen,
                    IsJuryActive = c.IsJuryActive,
                    IsScoreboardLive = c.IsScoreboardLive,
                    IsCertificatesPublished = c.IsCertificatesPublished,
                    PartnerName = c.Partner?.PartnerName,
                    Stages = c.Stages.OrderBy(s => s.DayNumber).ThenBy(s => s.StartTime).Select(s => new CompetitionStageDetailsDto
                    {
                        Id = s.Id,
                        DayNumber = s.DayNumber,
                        Title = s.Title,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsCompleted = s.IsCompleted
                    }).ToList()
                };
            },
            tags: new[] { "competitions" },
            cancellationToken: cancellationToken);

        return details;
    }
}

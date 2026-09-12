using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories.Competitions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetCompetitionDetails;

public class GetCompetitionDetailsQuery : IRequest<object?>
{
    public Guid Id { get; set; }
}

public class GetCompetitionDetailsQueryHandler : IRequestHandler<GetCompetitionDetailsQuery, object?>
{
    private readonly ICompetitionRepository _competitionRepository;

    public GetCompetitionDetailsQueryHandler(ICompetitionRepository competitionRepository)
    {
        _competitionRepository = competitionRepository;
    }

    public async Task<object?> Handle(GetCompetitionDetailsQuery request, CancellationToken cancellationToken)
    {
        var c = await _competitionRepository.GetAsync(
            x => x.Id == request.Id,
            q => q.Include(x => x.Partner).Include(x => x.Stages),
            cancellationToken);

        if (c == null)
            return null;

        return new
        {
            c.Id,
            c.Title,
            c.ShortSummary,
            c.Description,
            c.StartDate,
            c.EndDate,
            c.RegistrationDeadline,
            c.SubmissionDeadline,
            c.Location,
            c.LocationMapLink,
            c.CoverImageUrl,
            c.ParticipationFormat,
            c.MaxTeamSize,
            c.Tags,
            c.EvaluationCriteria,
            c.ContactEmail,
            c.ContactPhone,
            c.ContactSocialLink,
            c.GitHubRepositoryRequirement,
            c.LiveDeploymentRequirement,
            c.PitchDeckFormat,
            c.AgendaMode,
            c.AgendaPdfUrl,
            c.IsPublished,
            c.IsRegistrationOpen,
            c.IsJuryActive,
            c.IsScoreboardLive,
            c.IsCertificatesPublished,
            PartnerName = c.Partner?.PartnerName,
            Stages = c.Stages.OrderBy(s => s.DayNumber).ThenBy(s => s.StartTime).Select(s => new
            {
                s.Id,
                s.DayNumber,
                s.Title,
                s.StartTime,
                s.EndTime,
                s.IsCompleted
            }).ToList()
        };
    }
}


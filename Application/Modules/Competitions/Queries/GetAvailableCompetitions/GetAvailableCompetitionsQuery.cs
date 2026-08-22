using MediatR;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Repositories;
using Application.Repositories.Competitions;


namespace Application.Modules.Competitions.Queries.GetAvailableCompetitions;

public class GetAvailableCompetitionsQuery : IRequest<object>
{
}

public class GetAvailableCompetitionsQueryHandler : IRequestHandler<GetAvailableCompetitionsQuery, object>
{
    private readonly ICompetitionRepository _competitionRepository;
    private readonly IPartnerProfileRepository _partnerProfileRepository;

    public GetAvailableCompetitionsQueryHandler(
        ICompetitionRepository competitionRepository,
        IPartnerProfileRepository partnerProfileRepository)
    {
        _competitionRepository = competitionRepository;
        _partnerProfileRepository = partnerProfileRepository;
    }

    public async Task<object> Handle(GetAvailableCompetitionsQuery request, CancellationToken cancellationToken)
    {
        var competitions = await _competitionRepository.GetAllAsync(c => c.IsPublished, cancellationToken);
        var partners = await _partnerProfileRepository.GetAllAsync(null, cancellationToken);

        var data = competitions
            .OrderBy(c => c.StartDate)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.ShortSummary,
                c.Description,
                c.StartDate,
                c.EndDate,
                c.RegistrationDeadline,
                c.Location,
                c.CoverImageUrl,
                c.ParticipationFormat,
                c.MaxTeamSize,
                PartnerName = partners.FirstOrDefault(p => p.Id == c.PartnerId)?.PartnerName
            })
            .ToList();

        return new { success = true, data = data };
    }
}

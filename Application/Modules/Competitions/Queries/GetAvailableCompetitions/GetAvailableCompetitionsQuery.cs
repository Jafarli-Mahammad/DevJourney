using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.Competitions.Dtos;
using Application.Repositories;
using Application.Repositories.Competitions;
using Microsoft.Extensions.Caching.Hybrid;

namespace Application.Modules.Competitions.Queries.GetAvailableCompetitions;

public class GetAvailableCompetitionsQuery : IRequest<object>
{
}

public class GetAvailableCompetitionsQueryHandler : IRequestHandler<GetAvailableCompetitionsQuery, object>
{
    private readonly ICompetitionRepository _competitionRepository;
    private readonly IPartnerProfileRepository _partnerProfileRepository;
    private readonly HybridCache _hybridCache;

    public GetAvailableCompetitionsQueryHandler(
        ICompetitionRepository competitionRepository,
        IPartnerProfileRepository partnerProfileRepository,
        HybridCache hybridCache)
    {
        _competitionRepository = competitionRepository;
        _partnerProfileRepository = partnerProfileRepository;
        _hybridCache = hybridCache;
    }

    public async Task<object> Handle(GetAvailableCompetitionsQuery request, CancellationToken cancellationToken)
    {
        var data = await _hybridCache.GetOrCreateAsync(
            "competitions:available",
            async token =>
            {
                var competitions = await _competitionRepository.GetAllAsync(
                    c => c.IsPublished,
                    q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(q, c => c.Partner),
                    token);

                return competitions
                    .OrderBy(c => c.StartDate)
                    .Select(c => new AvailableCompetitionDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        ShortSummary = c.ShortSummary,
                        Description = c.Description,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        RegistrationDeadline = c.RegistrationDeadline,
                        Location = c.Location,
                        CoverImageUrl = c.CoverImageUrl,
                        ParticipationFormat = c.ParticipationFormat,
                        MaxTeamSize = c.MaxTeamSize,
                        PartnerName = c.Partner?.PartnerName
                    })
                    .ToList();
            },
            tags: new[] { "competitions" },
            cancellationToken: cancellationToken);

        return new { success = true, data };
    }
}

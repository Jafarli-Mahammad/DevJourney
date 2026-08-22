using Application.Exceptions;
using Application.Repositories;
using Application.Repositories.Competitions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetCompetitionById
{
    public class GetCompetitionByIdQuery : IRequest<object>
    {
        public Guid CompetitionId { get; set; }
    }

    public class GetCompetitionByIdQueryHandler : IRequestHandler<GetCompetitionByIdQuery, object>
    {
        private readonly ICompetitionRepository _competitionRepo;

        public GetCompetitionByIdQueryHandler(ICompetitionRepository competitionRepo)
        {
            _competitionRepo = competitionRepo;
        }

        public async Task<object> Handle(GetCompetitionByIdQuery request, CancellationToken cancellationToken)
        {
            var competition = await _competitionRepo.GetAsync(c => c.Id == request.CompetitionId, null, cancellationToken);
            if (competition == null)
                throw new NotFoundException("Competition", request.CompetitionId);

            return competition;
        }
    }
}

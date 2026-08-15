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
        public Task<object> Handle(GetCompetitionByIdQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(new { success = true, data = new object() });
        }
    }
}

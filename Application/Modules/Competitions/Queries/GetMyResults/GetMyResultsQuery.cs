using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetMyResults;

public class GetMyResultsQuery : IRequest<object>
{
    public int CompetitionId { get; set; }
}

public class GetMyResultsQueryHandler : IRequestHandler<GetMyResultsQuery, object>
{
    public Task<object> Handle(GetMyResultsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new { Score = 100 } });
    }
}

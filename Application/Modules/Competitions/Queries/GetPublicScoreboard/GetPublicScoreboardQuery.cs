using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetPublicScoreboard;

public class GetPublicScoreboardQuery : IRequest<object>
{
}

public class GetPublicScoreboardQueryHandler : IRequestHandler<GetPublicScoreboardQuery, object>
{
    public Task<object> Handle(GetPublicScoreboardQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new object[0] });
    }
}

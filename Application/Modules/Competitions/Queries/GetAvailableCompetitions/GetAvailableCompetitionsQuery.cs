using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetAvailableCompetitions;

public class GetAvailableCompetitionsQuery : IRequest<object>
{
}

public class GetAvailableCompetitionsQueryHandler : IRequestHandler<GetAvailableCompetitionsQuery, object>
{
    public Task<object> Handle(GetAvailableCompetitionsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new object[0] });
    }
}

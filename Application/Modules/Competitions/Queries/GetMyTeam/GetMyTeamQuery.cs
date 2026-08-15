using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetMyTeam;

public class GetMyTeamQuery : IRequest<object>
{
    public Guid CompetitionId { get; set; }
}

public class GetMyTeamQueryHandler : IRequestHandler<GetMyTeamQuery, object>
{
    public Task<object> Handle(GetMyTeamQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new { TeamName = "My Team" } });
    }
}

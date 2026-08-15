using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetCompetitionDetails;

public class GetCompetitionDetailsQuery : IRequest<object>
{
    public Guid Id { get; set; }
}

public class GetCompetitionDetailsQueryHandler : IRequestHandler<GetCompetitionDetailsQuery, object>
{
    public Task<object> Handle(GetCompetitionDetailsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new { Id = request.Id, Title = "Sample Competition" } });
    }
}

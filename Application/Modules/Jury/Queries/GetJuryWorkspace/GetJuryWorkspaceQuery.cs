using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Queries.GetJuryWorkspace
{
    public class GetJuryWorkspaceQuery : IRequest<object>
    {
        public Guid CompetitionId { get; set; }
    }

    public class GetJuryWorkspaceQueryHandler : IRequestHandler<GetJuryWorkspaceQuery, object>
    {
        public Task<object> Handle(GetJuryWorkspaceQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(new { success = true, data = Array.Empty<object>() });
        }
    }
}

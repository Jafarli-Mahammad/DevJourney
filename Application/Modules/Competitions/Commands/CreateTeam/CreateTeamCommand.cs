using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.CreateTeam;

public class CreateTeamCommand : IRequest<object>
{
    public Guid CompetitionId { get; set; }
    public string TeamName { get; set; } = string.Empty;
}

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, object>
{
    public Task<object> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new { TeamId = 1, request.TeamName } });
    }
}

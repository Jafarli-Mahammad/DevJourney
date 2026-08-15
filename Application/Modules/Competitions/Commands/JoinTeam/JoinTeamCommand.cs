using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.JoinTeam;

public class JoinTeamCommand : IRequest<object>
{
    public int CompetitionId { get; set; }
    public string InviteCode { get; set; } = string.Empty;
}

public class JoinTeamCommandHandler : IRequestHandler<JoinTeamCommand, object>
{
    public Task<object> Handle(JoinTeamCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new { Message = "Joined successfully" } });
    }
}

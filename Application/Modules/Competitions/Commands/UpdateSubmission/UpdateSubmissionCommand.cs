using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Commands.UpdateSubmission;

public class UpdateSubmissionCommand : IRequest<object>
{
    public int CompetitionId { get; set; }
    public string? GithubUrl { get; set; }
    public string? PitchDeckAssetId { get; set; }
}

public class UpdateSubmissionCommandHandler : IRequestHandler<UpdateSubmissionCommand, object>
{
    public Task<object> Handle(UpdateSubmissionCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new { Message = "Submission updated" } });
    }
}

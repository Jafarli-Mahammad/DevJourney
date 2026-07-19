using MediatR;

namespace Application.Modules.Posts.Commands.CreateTeamSearchPost
{
    public record CreateTeamSearchPostCommand(
        string? Note,
        List<Guid> IdeaFieldIds
    ) : IRequest<CreateTeamSearchPostCommandResponse>;

    public record CreateTeamSearchPostCommandResponse(Guid Id);
}

using Application.Modules.Posts.Queries.GetTeamMemberSearchPost;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamMemberSearchPost
{
    public record GetTeamMemberSearchPostDetailQuery(Guid Id) : IRequest<TeamMemberSearchPostDetailDto>;
}

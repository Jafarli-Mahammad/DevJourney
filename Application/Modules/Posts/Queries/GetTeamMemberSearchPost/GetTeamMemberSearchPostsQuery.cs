using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamMemberSearchPost
{
    public record GetTeamMemberSearchPostsQuery(TeamMemberSearchPostPagedFilter Filter)
        : IRequest<PagedResult<TeamMemberSearchPostPagedItemDto>>;
}

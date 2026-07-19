using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamSearchPost
{
    public record GetTeamSearchPostsQuery(TeamSearchPostPagedFilter Filter)
        : IRequest<PagedResult<TeamSearchPostPagedItemDto>>;
}

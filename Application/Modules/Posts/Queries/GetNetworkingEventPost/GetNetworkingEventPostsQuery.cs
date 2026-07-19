using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public record GetNetworkingEventPostsQuery(NetworkingEventPostPagedFilter Filter)
        : IRequest<PagedResult<NetworkingEventPostPagedItemDto>>;
}

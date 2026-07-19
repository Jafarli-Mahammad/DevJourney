using Application.Repositories;
using Application.Repositories.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public class GetNetworkingEventPostsQueryHandler : IRequestHandler<GetNetworkingEventPostsQuery, PagedResult<NetworkingEventPostPagedItemDto>>
    {
        private readonly INetworkingEventPostRepository _networkingEventPostRepository;

        public GetNetworkingEventPostsQueryHandler(INetworkingEventPostRepository networkingEventPostRepository)
        {
            _networkingEventPostRepository = networkingEventPostRepository;
        }

        public Task<PagedResult<NetworkingEventPostPagedItemDto>> Handle(GetNetworkingEventPostsQuery request, CancellationToken cancellationToken)
        {
            return _networkingEventPostRepository.GetPagedAsync(request.Filter, cancellationToken);
        }
    }
}

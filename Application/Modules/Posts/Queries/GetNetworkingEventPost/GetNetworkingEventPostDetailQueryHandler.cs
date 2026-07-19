using Application.Repositories.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public class GetNetworkingEventPostDetailQueryHandler : IRequestHandler<GetNetworkingEventPostDetailQuery, NetworkingEventPostDetailDto>
    {
        private readonly INetworkingEventPostRepository _networkingEventPostRepository;

        public GetNetworkingEventPostDetailQueryHandler(INetworkingEventPostRepository networkingEventPostRepository)
        {
            _networkingEventPostRepository = networkingEventPostRepository;
        }

        public Task<NetworkingEventPostDetailDto> Handle(GetNetworkingEventPostDetailQuery request, CancellationToken cancellationToken)
        {
            return _networkingEventPostRepository.GetDetailAsync(request.Id, cancellationToken);
        }
    }
}

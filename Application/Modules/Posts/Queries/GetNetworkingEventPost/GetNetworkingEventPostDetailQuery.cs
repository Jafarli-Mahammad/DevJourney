using MediatR;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public record GetNetworkingEventPostDetailQuery(Guid Id) : IRequest<NetworkingEventPostDetailDto>;
}

using Application.Modules.Posts.Queries.GetNetworkingEventPost;
using Application.Repositories;

namespace Application.Repositories.Post
{
    public interface INetworkingEventPostRepository : IAsyncRepository<Domain.Models.Entities.Post.NetworkingEventPost>
    {
        Task<NetworkingEventPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default);
        Task<PagedResult<NetworkingEventPostPagedItemDto>> GetPagedAsync(NetworkingEventPostPagedFilter filter, CancellationToken ct = default);
    }
}

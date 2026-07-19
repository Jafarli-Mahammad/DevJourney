using Application.Modules.Posts.Queries.GetB2BCoursePromoPost;
using Application.Repositories;

namespace Application.Repositories.Post
{
    public interface IB2BCoursePromoPostRepository : IAsyncRepository<Domain.Models.Entities.Post.B2BCoursePromoPost>
    {
        Task<B2BCoursePromoPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default);
        Task<PagedResult<B2BCoursePromoPostPagedItemDto>> GetPagedAsync(B2BCoursePromoPostPagedFilter filter, CancellationToken ct = default);
    }
}

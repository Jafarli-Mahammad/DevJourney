using Application.Modules.Posts.Queries.GetCorporateEventPost;
using Application.Repositories;

namespace Application.Repositories.Post
{
    public interface ICorporateEventPostRepository : IAsyncRepository<Domain.Models.Entities.Post.CorporateEventPost>
    {
        Task<CorporateEventPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default);
        Task<PagedResult<CorporateEventPostPagedItemDto>> GetPagedAsync(CorporateEventPostPagedFilter filter, CancellationToken ct = default);
    }
}

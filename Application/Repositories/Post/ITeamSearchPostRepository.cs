using Domain.Models.Entities.Post;

namespace Application.Repositories
{
    public interface ITeamSearchPostRepository : IAsyncRepository<TeamSearchPost>
    {
        Task<PagedResult<TeamSearchPostPagedItemDto>> GetPagedAsync(TeamSearchPostPagedFilter filter, CancellationToken ct = default);
        Task<TeamSearchPost?> GetDetailAsync(Guid postId, CancellationToken ct = default);
    }
}
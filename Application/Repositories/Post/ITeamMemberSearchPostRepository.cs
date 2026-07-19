using Application.Modules.Posts.Queries.GetTeamMemberSearchPost;
using Domain.Models.Entities.Post;

namespace Application.Repositories
{
    public interface ITeamMemberSearchPostRepository : IAsyncRepository<TeamMemberSearchPost>
    {
        Task<PagedResult<TeamMemberSearchPostPagedItemDto>> GetPagedAsync(
            TeamMemberSearchPostPagedFilter filter, CancellationToken ct = default);

        Task<TeamMemberSearchPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default);
    }
}
using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamMemberSearchPost
{

    public class GetTeamMemberSearchPostsQueryHandler
        : IRequestHandler<GetTeamMemberSearchPostsQuery, PagedResult<TeamMemberSearchPostPagedItemDto>>
    {
        private readonly ITeamMemberSearchPostRepository _repository;
        public GetTeamMemberSearchPostsQueryHandler(ITeamMemberSearchPostRepository repository)
            => _repository = repository;

        public Task<PagedResult<TeamMemberSearchPostPagedItemDto>> Handle(
            GetTeamMemberSearchPostsQuery request, CancellationToken ct)
            => _repository.GetPagedAsync(request.Filter, ct);
    }
}

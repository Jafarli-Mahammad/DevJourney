using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamSearchPost
{
    public class GetTeamSearchPostsQueryHandler : IRequestHandler<GetTeamSearchPostsQuery, PagedResult<TeamSearchPostPagedItemDto>>
    {
        private readonly ITeamSearchPostRepository _teamSearchPostRepository;

        public GetTeamSearchPostsQueryHandler(ITeamSearchPostRepository teamSearchPostRepository)
        {
            _teamSearchPostRepository = teamSearchPostRepository;
        }

        public Task<PagedResult<TeamSearchPostPagedItemDto>> Handle(GetTeamSearchPostsQuery request, CancellationToken cancellationToken)
        {
            return _teamSearchPostRepository.GetPagedAsync(request.Filter, cancellationToken);
        }
    }
}

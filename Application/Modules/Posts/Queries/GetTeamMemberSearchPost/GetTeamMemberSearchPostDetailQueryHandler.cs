using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamMemberSearchPost
{
    public class GetTeamMemberSearchPostDetailQueryHandler : IRequestHandler<GetTeamMemberSearchPostDetailQuery, TeamMemberSearchPostDetailDto>
    {
        private readonly ITeamMemberSearchPostRepository _teamMemberSearchPostRepository;

        public GetTeamMemberSearchPostDetailQueryHandler(ITeamMemberSearchPostRepository teamMemberSearchPostRepository)
        {
            _teamMemberSearchPostRepository = teamMemberSearchPostRepository;
        }

        public Task<TeamMemberSearchPostDetailDto> Handle(GetTeamMemberSearchPostDetailQuery request, CancellationToken cancellationToken)
        {
            return _teamMemberSearchPostRepository.GetDetailAsync(request.Id, cancellationToken);
        }
    }
}

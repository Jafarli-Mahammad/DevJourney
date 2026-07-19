using Application.Repositories;
using Domain.Models.Entities.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamSearchPost
{
    public class GetTeamSearchPostDetailQueryHandler : IRequestHandler<GetTeamSearchPostDetailQuery, TeamSearchPost?>
    {
        private readonly ITeamSearchPostRepository _teamSearchPostRepository;

        public GetTeamSearchPostDetailQueryHandler(ITeamSearchPostRepository teamSearchPostRepository)
        {
            _teamSearchPostRepository = teamSearchPostRepository;
        }

        public Task<TeamSearchPost?> Handle(GetTeamSearchPostDetailQuery request, CancellationToken cancellationToken)
        {
            return _teamSearchPostRepository.GetDetailAsync(request.Id, cancellationToken);
        }
    }
}

using Application.Modules.Posts.Notifications;
using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Post;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateTeamSearchPost
{
    public class CreateTeamSearchPostCommandHandler : IRequestHandler<CreateTeamSearchPostCommand, CreateTeamSearchPostCommandResponse>
    {
        private readonly ITeamSearchPostRepository _teamSearchPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CreateTeamSearchPostCommandHandler(
            ITeamSearchPostRepository teamSearchPostRepository,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _teamSearchPostRepository = teamSearchPostRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<CreateTeamSearchPostCommandResponse> Handle(CreateTeamSearchPostCommand request, CancellationToken cancellationToken)
        {
            var post = new TeamSearchPost
            {
                Id = Guid.NewGuid(),
                AuthorId = _currentUserService.UserId,
                Type = PostType.TeamSearch,
                Note = request.Note,
                IdeaFields = request.IdeaFieldIds
                    .Select(id => new TeamSearchPostIdeaField { IdeaFieldId = id })
                    .ToList()
            };

            await _teamSearchPostRepository.AddAsync(post, cancellationToken);
            await _mediator.Publish(new PostCreatedNotification(post.Id, post.AuthorId, post.Type), cancellationToken);

            return new CreateTeamSearchPostCommandResponse(post.Id);
        }
    }
}

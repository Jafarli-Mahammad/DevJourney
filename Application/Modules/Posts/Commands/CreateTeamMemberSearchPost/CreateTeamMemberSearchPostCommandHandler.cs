using Application.Modules.Posts.Notifications;
using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Post;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateTeamMemberSearchPost
{
    public class CreateTeamMemberSearchPostCommandHandler : IRequestHandler<CreateTeamMemberSearchPostCommand, CreateTeamMemberSearchPostCommandResponse>
    {
        private readonly ITeamMemberSearchPostRepository teamMemberSearchPostRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IMediator mediator;

        public CreateTeamMemberSearchPostCommandHandler(ITeamMemberSearchPostRepository teamMemberSearchPostRepository, ICurrentUserService currentUserService, IMediator mediator)
        {
            this.teamMemberSearchPostRepository = teamMemberSearchPostRepository;
            this.currentUserService = currentUserService;
            this.mediator = mediator;
        }

        public async Task<CreateTeamMemberSearchPostCommandResponse> Handle(CreateTeamMemberSearchPostCommand request, CancellationToken cancellationToken)
        {
            var post = new TeamMemberSearchPost
            {
                Id = Guid.NewGuid(),
                AuthorId = currentUserService.UserId,
                Type = PostType.TeamMemberSearch,
                IdeaFieldId = request.IdeaFieldId,
                TeamName = request.TeamName,
                MembersNeeded = request.MembersNeeded,
                WorkMode = request.WorkMode,
                SocialLink = request.SocialLink,
                LookingForAge = request.LookingForAge,
                LookingForLocation = request.LookingForLocation,
                LookingForLanguageId = request.LookingForLanguageId,
                AdditionalNote = request.AdditionalNote,
                TargetRoles = request.TargetRoleIds
                .Select(id => new TeamMemberSearchPostRole { RoleId = id }).ToList(),
                TargetSkills = request.TargetSkillIds
                .Select(id => new TeamMemberSearchPostSkill { SkillId = id }).ToList()
            };

            await teamMemberSearchPostRepository.AddAsync(post, cancellationToken);
            await mediator.Publish(new PostCreatedNotification(post.Id, post.AuthorId, post.Type), cancellationToken);

            return new CreateTeamMemberSearchPostCommandResponse(post.Id);
        }
    }
}

using FluentValidation;

namespace Application.Modules.Posts.Commands.CreateTeamMemberSearchPost
{
    public class CreateTeamMemberSearchPostCommandValidator
        : AbstractValidator<CreateTeamMemberSearchPostCommand>
    {
        public CreateTeamMemberSearchPostCommandValidator()
        {
            RuleFor(x => x.IdeaFieldId).NotEmpty();
            RuleFor(x => x.TeamName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.MembersNeeded).GreaterThan(0);
            RuleFor(x => x.SocialLink).NotEmpty();
            RuleFor(x => x.AdditionalNote).MaximumLength(100);
            RuleFor(x => x.TargetRoleIds).NotEmpty();
            RuleFor(x => x.TargetSkillIds).NotEmpty();
            RuleFor(x => x.LookingForAge).InclusiveBetween(14, 99)
                .When(x => x.LookingForAge.HasValue);
        }
    }
}

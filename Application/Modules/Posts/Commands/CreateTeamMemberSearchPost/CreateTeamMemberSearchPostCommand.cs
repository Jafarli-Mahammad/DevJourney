using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateTeamMemberSearchPost
{
    public record CreateTeamMemberSearchPostCommand(
        Guid IdeaFieldId,
        string TeamName,
        int MembersNeeded,
        WorkFormat WorkMode,
        string SocialLink,
        int? LookingForAge,
        string? LookingForLocation,
        Guid? LookingForLanguageId,
        string? AdditionalNote,
        List<Guid> TargetRoleIds,
        List<Guid> TargetSkillIds
    ) : IRequest<CreateTeamMemberSearchPostCommandResponse>;
}
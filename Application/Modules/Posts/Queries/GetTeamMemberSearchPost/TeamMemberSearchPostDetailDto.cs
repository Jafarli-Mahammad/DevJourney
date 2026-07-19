using Domain.Models.Enums;

namespace Application.Modules.Posts.Queries.GetTeamMemberSearchPost
{
    public record TeamMemberSearchPostDetailDto(
        Guid Id,
        Guid AuthorId,
        string AuthorName,
        string? AuthorAvatarUrl,
        string TeamName,
        Guid IdeaFieldId,
        string IdeaFieldName,
        int MembersNeeded,
        WorkFormat WorkMode,
        string SocialLink,
        int? LookingForAge,
        string? LookingForLocation,
        string? LookingForLanguageName,
        string? AdditionalNote,
        IReadOnlyList<LookupItemDto> TargetRoles,
        IReadOnlyList<LookupItemDto> TargetSkills,
        bool IsEdited,
        DateTime CreatedAt
    );
}

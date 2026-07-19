namespace Application.Repositories
{
    public sealed record TeamMemberSearchPostPagedFilter(
        IReadOnlyCollection<Guid>? RoleIds = null,
        IReadOnlyCollection<Guid>? SkillIds = null,
        Guid? IdeaFieldId = null,
        string? Location = null,
        Guid? LanguageId = null,
        int Page = 1,
        int PageSize = 10);
}
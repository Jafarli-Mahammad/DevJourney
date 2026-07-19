namespace Application.Repositories
{
    public sealed class TeamMemberSearchPostPagedItemDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid IdeaFieldId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int MembersNeeded { get; set; }
        public string WorkMode { get; set; } = string.Empty;
        public string SocialLink { get; set; } = string.Empty;
        public int? LookingForAge { get; set; }
        public string? LookingForLocation { get; set; }
        public Guid? LookingForLanguageId { get; set; }
        public string? LookingForLanguageName { get; set; }
        public string? AdditionalNote { get; set; }
        public IReadOnlyCollection<Guid> RoleIds { get; set; } = Array.Empty<Guid>();
        public IReadOnlyCollection<Guid> SkillIds { get; set; } = Array.Empty<Guid>();
    }
}
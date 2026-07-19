namespace Application.Repositories
{
    public sealed class TeamSearchPostPagedItemDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string? Note { get; set; }
        public IReadOnlyCollection<Guid> IdeaFieldIds { get; set; } = Array.Empty<Guid>();
        public string? StudentLocation { get; set; }
        public string? StudentExperience { get; set; }
        public string? StudentRole { get; set; }
        public IReadOnlyCollection<Guid> StudentSkillIds { get; set; } = Array.Empty<Guid>();
        public IReadOnlyCollection<Guid> StudentLanguageIds { get; set; } = Array.Empty<Guid>();
    }
}
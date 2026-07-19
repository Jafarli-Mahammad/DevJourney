namespace Application.Repositories
{
    public sealed record TeamSearchPostPagedFilter(
        IReadOnlyCollection<Guid>? IdeaFieldIds = null,
        int Page = 1,
        int PageSize = 10);
}
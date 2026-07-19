using Application.Repositories;
using Dapper;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class TeamSearchPostRepository : AsyncRepository<TeamSearchPost>, ITeamSearchPostRepository
    {
        public TeamSearchPostRepository(DataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<PagedResult<TeamSearchPostPagedItemDto>> GetPagedAsync(TeamSearchPostPagedFilter filter, CancellationToken ct = default)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;
            var offset = (page - 1) * pageSize;
            var ideaFieldIds = PrepareIds(filter.IdeaFieldIds);
            var parameters = new DynamicParameters();
            parameters.Add("IdeaFieldIds", ideaFieldIds);
            parameters.Add("HasIdeaFieldFilter", filter.IdeaFieldIds is not null && filter.IdeaFieldIds.Any());
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var connection = DataContext.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;

            if (shouldClose)
            {
                await connection.OpenAsync(ct);
            }

            try
            {
                var countSql = """
                    SELECT COUNT(1)
                    FROM Posts.Posts p
                    INNER JOIN Posts.TeamSearchPosts tsp ON tsp.Id = p.Id
                    WHERE (@HasIdeaFieldFilter = 0 OR EXISTS (
                        SELECT 1
                        FROM Posts.TeamSearchPostIdeaFields tf
                        WHERE tf.PostId = p.Id AND tf.IdeaFieldId IN @IdeaFieldIds));
                    """;

                var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: ct));

                var dataSql = """
                    SELECT p.Id,
                           p.AuthorId,
                           tsp.Note,
                           ideaFields.IdeaFieldIdsCsv,
                           sp.Location AS StudentLocation,
                           CONVERT(nvarchar(50), sp.Experience) AS StudentExperience,
                           CAST(NULL AS nvarchar(50)) AS StudentRole,
                           skills.SkillIdsCsv,
                           languages.LanguageIdsCsv
                    FROM Posts.Posts p
                    INNER JOIN Posts.TeamSearchPosts tsp ON tsp.Id = p.Id
                    LEFT JOIN Student.StudentProfiles sp ON sp.ApplicationUserId = p.AuthorId
                    OUTER APPLY (
                        SELECT STRING_AGG(CONVERT(varchar(36), tf.IdeaFieldId), ',') AS IdeaFieldIdsCsv
                        FROM Posts.TeamSearchPostIdeaFields tf
                        WHERE tf.PostId = p.Id
                    ) ideaFields
                    OUTER APPLY (
                        SELECT STRING_AGG(CONVERT(varchar(36), ss.SkillId), ',') AS SkillIdsCsv
                        FROM Student.StudentSkills ss
                        WHERE sp.Id IS NOT NULL AND ss.StudentProfileId = sp.Id
                    ) skills
                    OUTER APPLY (
                        SELECT STRING_AGG(CONVERT(varchar(36), sl.LanguageId), ',') AS LanguageIdsCsv
                        FROM Student.StudentLanguages sl
                        WHERE sp.Id IS NOT NULL AND sl.StudentProfileId = sp.Id
                    ) languages
                    WHERE (@HasIdeaFieldFilter = 0 OR EXISTS (
                        SELECT 1
                        FROM Posts.TeamSearchPostIdeaFields tf
                        WHERE tf.PostId = p.Id AND tf.IdeaFieldId IN @IdeaFieldIds))
                    ORDER BY p.CreatedAt DESC, p.Id DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                    """;

                var rows = await connection.QueryAsync<TeamSearchPostPagedRow>(new CommandDefinition(dataSql, parameters, cancellationToken: ct));

                var items = rows.Select(row => new TeamSearchPostPagedItemDto
                {
                    Id = row.Id,
                    AuthorId = row.AuthorId,
                    Note = row.Note,
                    IdeaFieldIds = ParseIds(row.IdeaFieldIdsCsv),
                    StudentLocation = row.StudentLocation,
                    StudentExperience = row.StudentExperience,
                    StudentRole = row.StudentRole,
                    StudentSkillIds = ParseIds(row.SkillIdsCsv),
                    StudentLanguageIds = ParseIds(row.LanguageIdsCsv)
                }).ToList();

                return new PagedResult<TeamSearchPostPagedItemDto>(items, totalCount, page, pageSize);
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<TeamSearchPost?> GetDetailAsync(Guid postId, CancellationToken ct = default)
        {
            return await GetAsync(
                post => post.Id == postId,
                query => query.Include(post => post.IdeaFields),
                ct);
        }

        private static Guid[] PrepareIds(IReadOnlyCollection<Guid>? values)
        {
            var ids = values?.Where(id => id != Guid.Empty).Distinct().ToArray();
            return ids is { Length: > 0 } ? ids : [Guid.Empty];
        }

        private static IReadOnlyCollection<Guid> ParseIds(string? values)
        {
            if (string.IsNullOrWhiteSpace(values))
            {
                return Array.Empty<Guid>();
            }

            return values
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => Guid.Parse(value))
                .ToArray();
        }

        private sealed class TeamSearchPostPagedRow
        {
            public Guid Id { get; set; }
            public Guid AuthorId { get; set; }
            public string? Note { get; set; }
            public string? IdeaFieldIdsCsv { get; set; }
            public string? StudentLocation { get; set; }
            public string? StudentExperience { get; set; }
            public string? StudentRole { get; set; }
            public string? SkillIdsCsv { get; set; }
            public string? LanguageIdsCsv { get; set; }
        }
    }
}
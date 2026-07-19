using Application.Modules.Posts.Queries.GetTeamMemberSearchPost;
using Application.Repositories;
using Dapper;
using DataAccessLayer.DataContexts;
using DataAccessLayer.IdentityEntities;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class TeamMemberSearchPostRepository : AsyncRepository<TeamMemberSearchPost>, ITeamMemberSearchPostRepository
    {
        public TeamMemberSearchPostRepository(DataContext dataContext)
            : base(dataContext)
        {
        }

        public async Task<TeamMemberSearchPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default)
        {
            var post = await GetAsync(
                post => post.Id == postId,
                query => query
                    .Include(post => post.IdeaField)
                    .Include(post => post.LookingForLanguage)
                    .Include(post => post.TargetRoles)
                        .ThenInclude(postRole => postRole.Role)
                    .Include(post => post.TargetSkills),
                ct);

            var postEntity = post!;

            var author = await DataContext.Users
                .AsNoTracking()
                .Where(user => user.Id == postEntity.AuthorId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync(ct);

            var skillIds = postEntity.TargetSkills.Select(skill => skill.SkillId).Distinct().ToArray();
            var skills = await DataContext.Skills
                .AsNoTracking()
                .Where(skill => skillIds.Contains(skill.Id))
                .Select(skill => new LookupItemDto(skill.Id, skill.Name))
                .ToListAsync(ct);

            return new TeamMemberSearchPostDetailDto(
                postEntity.Id,
                postEntity.AuthorId,
                author ?? string.Empty,
                null,
                postEntity.TeamName,
                postEntity.IdeaFieldId,
                postEntity.IdeaField.Name,
                postEntity.MembersNeeded,
                postEntity.WorkMode,
                postEntity.SocialLink,
                postEntity.LookingForAge,
                postEntity.LookingForLocation,
                postEntity.LookingForLanguage?.Name,
                postEntity.AdditionalNote,
                postEntity.TargetRoles
                    .Select(postRole => new LookupItemDto(postRole.RoleId, postRole.Role.Name))
                    .ToList(),
                skills,
                postEntity.IsEdited,
                postEntity.CreatedAt);
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

        public async Task<PagedResult<TeamMemberSearchPostPagedItemDto>> GetPagedAsync(TeamMemberSearchPostPagedFilter filter, CancellationToken ct = default)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;
            var offset = (page - 1) * pageSize;
            var roleIds = PrepareIds(filter.RoleIds);
            var skillIds = PrepareIds(filter.SkillIds);
            var parameters = new DynamicParameters();
            parameters.Add("IdeaFieldId", filter.IdeaFieldId);
            parameters.Add("Location", string.IsNullOrWhiteSpace(filter.Location) ? null : filter.Location);
            parameters.Add("LanguageId", filter.LanguageId);
            parameters.Add("RoleIds", roleIds);
            parameters.Add("SkillIds", skillIds);
            parameters.Add("HasRoleFilter", filter.RoleIds is not null && filter.RoleIds.Any());
            parameters.Add("HasSkillFilter", filter.SkillIds is not null && filter.SkillIds.Any());
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
                    INNER JOIN Posts.TeamMemberSearchPosts tmsp ON tmsp.Id = p.Id
                                        WHERE p.DeletedAt IS NULL
                                            AND (@IdeaFieldId IS NULL OR tmsp.IdeaFieldId = @IdeaFieldId)
                      AND (@Location IS NULL OR tmsp.LookingForLocation LIKE '%' + @Location + '%')
                      AND (@LanguageId IS NULL OR tmsp.LookingForLanguageId = @LanguageId)
                      AND (@HasRoleFilter = 0 OR EXISTS (
                            SELECT 1
                            FROM Posts.TeamMemberSearchPostRoles r
                            WHERE r.PostId = p.Id AND r.RoleId IN @RoleIds))
                      AND (@HasSkillFilter = 0 OR EXISTS (
                            SELECT 1
                            FROM Posts.TeamMemberSearchPostSkills s
                            WHERE s.PostId = p.Id AND s.SkillId IN @SkillIds));
                    """;

                var totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: ct));

                var dataSql = """
                    SELECT p.Id,
                           p.AuthorId,
                           tmsp.IdeaFieldId,
                           tmsp.TeamName,
                           tmsp.MembersNeeded,
                           tmsp.WorkMode,
                           tmsp.SocialLink,
                           tmsp.LookingForAge,
                           tmsp.LookingForLocation,
                           tmsp.LookingForLanguageId,
                           lang.Name AS LookingForLanguageName,
                           tmsp.AdditionalNote,
                           p.CreatedAt,
                           roles.RoleIdsCsv,
                           skills.SkillIdsCsv
                    FROM Posts.Posts p
                    INNER JOIN Posts.TeamMemberSearchPosts tmsp ON tmsp.Id = p.Id
                    LEFT JOIN Student.Languages lang ON lang.Id = tmsp.LookingForLanguageId
                    OUTER APPLY (
                        SELECT STRING_AGG(CONVERT(varchar(36), r.RoleId), ',') AS RoleIdsCsv
                        FROM Posts.TeamMemberSearchPostRoles r
                        WHERE r.PostId = p.Id
                    ) roles
                    OUTER APPLY (
                        SELECT STRING_AGG(CONVERT(varchar(36), s.SkillId), ',') AS SkillIdsCsv
                        FROM Posts.TeamMemberSearchPostSkills s
                        WHERE s.PostId = p.Id
                    ) skills
                                        WHERE p.DeletedAt IS NULL
                                            AND (@IdeaFieldId IS NULL OR tmsp.IdeaFieldId = @IdeaFieldId)
                      AND (@Location IS NULL OR tmsp.LookingForLocation LIKE '%' + @Location + '%')
                      AND (@LanguageId IS NULL OR tmsp.LookingForLanguageId = @LanguageId)
                      AND (@HasRoleFilter = 0 OR EXISTS (
                            SELECT 1
                            FROM Posts.TeamMemberSearchPostRoles r
                            WHERE r.PostId = p.Id AND r.RoleId IN @RoleIds))
                      AND (@HasSkillFilter = 0 OR EXISTS (
                            SELECT 1
                            FROM Posts.TeamMemberSearchPostSkills s
                            WHERE s.PostId = p.Id AND s.SkillId IN @SkillIds))
                    ORDER BY p.CreatedAt DESC, p.Id DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                    """;

                var rows = await connection.QueryAsync<TeamMemberSearchPostPagedRow>(new CommandDefinition(dataSql, parameters, cancellationToken: ct));

                var items = rows.Select(row => new TeamMemberSearchPostPagedItemDto
                {
                    Id = row.Id,
                    AuthorId = row.AuthorId,
                    IdeaFieldId = row.IdeaFieldId,
                    TeamName = row.TeamName,
                    MembersNeeded = row.MembersNeeded,
                    WorkMode = row.WorkMode,
                    SocialLink = row.SocialLink,
                    LookingForAge = row.LookingForAge,
                    LookingForLocation = row.LookingForLocation,
                    LookingForLanguageId = row.LookingForLanguageId,
                    LookingForLanguageName = row.LookingForLanguageName,
                    AdditionalNote = row.AdditionalNote,
                    RoleIds = ParseIds(row.RoleIdsCsv),
                    SkillIds = ParseIds(row.SkillIdsCsv)
                }).ToList();

                return new PagedResult<TeamMemberSearchPostPagedItemDto>(items, totalCount, page, pageSize);
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private sealed class TeamMemberSearchPostPagedRow
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
            public string? RoleIdsCsv { get; set; }
            public string? SkillIdsCsv { get; set; }
        }
    }
}
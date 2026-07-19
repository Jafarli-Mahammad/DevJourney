using Application.Modules.Posts.Queries.GetCorporateEventPost;
using Application.Repositories;
using Application.Repositories.Post;
using Dapper;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataAccessLayer.Repositories.Post
{
    public class CorporateEventPostRepository : DapperPagedRepositoryBase<CorporateEventPost>, ICorporateEventPostRepository
    {
        public CorporateEventPostRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public async Task<CorporateEventPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default)
        {
            var post = await GetAsync(
                p => p.Id == postId,
                query => query.Include(p => p.EventType).Include(p => p.Agenda),
                ct);

            var postEntity = post!;

            var author = await DataContext.Users
                .AsNoTracking()
                .Where(user => user.Id == postEntity.AuthorId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync(ct);

            return new CorporateEventPostDetailDto(
                postEntity.Id,
                postEntity.AuthorId,
                author ?? string.Empty,
                postEntity.Title,
                postEntity.EventTypeId,
                postEntity.EventType.Name,
                postEntity.SpecialOccasion,
                postEntity.StartAt,
                postEntity.EndAt,
                postEntity.Location,
                postEntity.Latitude,
                postEntity.Longitude,
                postEntity.TargetAudience,
                postEntity.MaxAttendees,
                postEntity.ConfidentialityNote,
                postEntity.ApplicationMethod,
                postEntity.ApplicationLink,
                postEntity.EventLanguage,
                postEntity.IsPaid,
                postEntity.Price,
                postEntity.Agenda.OrderBy(a => a.Order).Select(a => new CorporateEventAgendaItemDto(a.Order, a.Time, a.Activity)).ToList(),
                postEntity.IsEdited,
                postEntity.CreatedAt
            );
        }

        public async Task<PagedResult<CorporateEventPostPagedItemDto>> GetPagedAsync(CorporateEventPostPagedFilter filter, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("EventTypeId", filter.EventTypeId);
            parameters.Add("CompanySector", string.IsNullOrWhiteSpace(filter.CompanySector) ? null : filter.CompanySector);
            parameters.Add("StartAtMin", filter.StartAtMin);
            parameters.Add("StartAtMax", filter.StartAtMax);
            parameters.Add("IsPaid", filter.IsPaid);
            parameters.Add("TargetMajor", string.IsNullOrWhiteSpace(filter.TargetMajor) ? null : filter.TargetMajor);

            var countSql = """
                SELECT COUNT(1)
                FROM Posts.Posts p
                INNER JOIN Posts.CorporateEventPosts cep ON cep.Id = p.Id
                LEFT JOIN Company.CompanyProfiles cp ON cp.ApplicationUserId = p.AuthorId
                WHERE p.DeletedAt IS NULL
                  AND (@EventTypeId IS NULL OR cep.EventTypeId = @EventTypeId)
                  AND (@CompanySector IS NULL OR cp.CompanySector LIKE '%' + @CompanySector + '%')
                  AND (@StartAtMin IS NULL OR cep.StartAt >= @StartAtMin)
                  AND (@StartAtMax IS NULL OR cep.StartAt <= @StartAtMax)
                  AND (@IsPaid IS NULL OR cep.IsPaid = @IsPaid)
                  AND (@TargetMajor IS NULL OR cep.TargetAudience LIKE '%' + @TargetMajor + '%');
                """;

            var dataSql = """
                SELECT p.Id,
                       p.AuthorId,
                       cep.Title,
                       cep.EventTypeId,
                       et.Name AS EventTypeName,
                       cep.StartAt,
                       cep.EndAt,
                       cep.Location,
                       cep.TargetAudience,
                       cep.MaxAttendees,
                       cep.IsPaid,
                       cep.Price,
                       p.CreatedAt
                FROM Posts.Posts p
                INNER JOIN Posts.CorporateEventPosts cep ON cep.Id = p.Id
                LEFT JOIN Posts.EventTypes et ON et.Id = cep.EventTypeId
                LEFT JOIN Company.CompanyProfiles cp ON cp.ApplicationUserId = p.AuthorId
                WHERE p.DeletedAt IS NULL
                  AND (@EventTypeId IS NULL OR cep.EventTypeId = @EventTypeId)
                  AND (@CompanySector IS NULL OR cp.CompanySector LIKE '%' + @CompanySector + '%')
                  AND (@StartAtMin IS NULL OR cep.StartAt >= @StartAtMin)
                  AND (@StartAtMax IS NULL OR cep.StartAt <= @StartAtMax)
                  AND (@IsPaid IS NULL OR cep.IsPaid = @IsPaid)
                  AND (@TargetMajor IS NULL OR cep.TargetAudience LIKE '%' + @TargetMajor + '%')
                ORDER BY p.CreatedAt DESC, p.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                """;

            return await GetPagedInternalAsync<CorporateEventPostPagedItemDto, CorporateEventPostPagedRow>(
                filter.Page,
                filter.PageSize,
                parameters,
                countSql,
                dataSql,
                row => new CorporateEventPostPagedItemDto
                {
                    Id = row.Id,
                    AuthorId = row.AuthorId,
                    Title = row.Title,
                    EventTypeId = row.EventTypeId,
                    EventTypeName = row.EventTypeName,
                    StartAt = row.StartAt,
                    EndAt = row.EndAt,
                    Location = row.Location,
                    TargetAudience = row.TargetAudience,
                    MaxAttendees = row.MaxAttendees,
                    IsPaid = row.IsPaid,
                    Price = row.Price,
                    CreatedAt = row.CreatedAt
                },
                ct);
        }

        private sealed class CorporateEventPostPagedRow
        {
            public Guid Id { get; set; }
            public Guid AuthorId { get; set; }
            public string Title { get; set; } = string.Empty;
            public Guid EventTypeId { get; set; }
            public string EventTypeName { get; set; } = string.Empty;
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public string Location { get; set; } = string.Empty;
            public string TargetAudience { get; set; } = string.Empty;
            public int MaxAttendees { get; set; }
            public bool IsPaid { get; set; }
            public decimal? Price { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

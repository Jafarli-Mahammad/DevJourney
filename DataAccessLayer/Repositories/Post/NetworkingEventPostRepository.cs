using Application.Modules.Posts.Queries.GetNetworkingEventPost;
using Application.Repositories;
using Application.Repositories.Post;
using Dapper;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataAccessLayer.Repositories.Post
{
    public class NetworkingEventPostRepository : DapperPagedRepositoryBase<NetworkingEventPost>, INetworkingEventPostRepository
    {
        public NetworkingEventPostRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public async Task<NetworkingEventPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default)
        {
            var post = await GetAsync(
                p => p.Id == postId,
                query => query.Include(p => p.Stops),
                ct);

            var postEntity = post!;

            var author = await DataContext.Users
                .AsNoTracking()
                .Where(user => user.Id == postEntity.AuthorId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync(ct);

            return new NetworkingEventPostDetailDto(
                postEntity.Id,
                postEntity.AuthorId,
                author ?? string.Empty,
                postEntity.OrganizationName,
                postEntity.Location,
                postEntity.Latitude,
                postEntity.Longitude,
                postEntity.StartAt,
                postEntity.EndAt,
                postEntity.MaxAttendees,
                postEntity.TicketContact,
                postEntity.IsPaid,
                postEntity.Price,
                postEntity.Stops.OrderBy(s => s.Order).Select(s => new NetworkingEventStopDto(s.Order, s.Description)).ToList(),
                postEntity.IsEdited,
                postEntity.CreatedAt
            );
        }

        public async Task<PagedResult<NetworkingEventPostPagedItemDto>> GetPagedAsync(NetworkingEventPostPagedFilter filter, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("IsPaid", filter.IsPaid);
            parameters.Add("Location", string.IsNullOrWhiteSpace(filter.Location) ? null : filter.Location);
            parameters.Add("StartAtMin", filter.StartAtMin);
            parameters.Add("StartAtMax", filter.StartAtMax);

            var countSql = """
                SELECT COUNT(1)
                FROM Posts.Posts p
                INNER JOIN Posts.NetworkingEventPosts nep ON nep.Id = p.Id
                WHERE p.DeletedAt IS NULL
                  AND (@IsPaid IS NULL OR nep.IsPaid = @IsPaid)
                  AND (@Location IS NULL OR nep.Location LIKE '%' + @Location + '%')
                  AND (@StartAtMin IS NULL OR nep.StartAt >= @StartAtMin)
                  AND (@StartAtMax IS NULL OR nep.StartAt <= @StartAtMax);
                """;

            var dataSql = """
                SELECT p.Id,
                       p.AuthorId,
                       nep.OrganizationName,
                       nep.Location,
                       nep.StartAt,
                       nep.EndAt,
                       nep.MaxAttendees,
                       nep.IsPaid,
                       nep.Price,
                       p.CreatedAt
                FROM Posts.Posts p
                INNER JOIN Posts.NetworkingEventPosts nep ON nep.Id = p.Id
                WHERE p.DeletedAt IS NULL
                  AND (@IsPaid IS NULL OR nep.IsPaid = @IsPaid)
                  AND (@Location IS NULL OR nep.Location LIKE '%' + @Location + '%')
                  AND (@StartAtMin IS NULL OR nep.StartAt >= @StartAtMin)
                  AND (@StartAtMax IS NULL OR nep.StartAt <= @StartAtMax)
                ORDER BY p.CreatedAt DESC, p.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                """;

            return await GetPagedInternalAsync<NetworkingEventPostPagedItemDto, NetworkingEventPostPagedRow>(
                filter.Page,
                filter.PageSize,
                parameters,
                countSql,
                dataSql,
                row => new NetworkingEventPostPagedItemDto
                {
                    Id = row.Id,
                    AuthorId = row.AuthorId,
                    OrganizationName = row.OrganizationName,
                    Location = row.Location,
                    StartAt = row.StartAt,
                    EndAt = row.EndAt,
                    MaxAttendees = row.MaxAttendees,
                    IsPaid = row.IsPaid,
                    Price = row.Price,
                    CreatedAt = row.CreatedAt
                },
                ct);
        }

        private sealed class NetworkingEventPostPagedRow
        {
            public Guid Id { get; set; }
            public Guid AuthorId { get; set; }
            public string OrganizationName { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public int MaxAttendees { get; set; }
            public bool IsPaid { get; set; }
            public decimal? Price { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

using Application.Modules.Posts.Queries.GetB2BCoursePromoPost;
using Application.Repositories;
using Application.Repositories.Post;
using Dapper;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Post;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataAccessLayer.Repositories.Post
{
    public class B2BCoursePromoPostRepository : DapperPagedRepositoryBase<B2BCoursePromoPost>, IB2BCoursePromoPostRepository
    {
        public B2BCoursePromoPostRepository(DataContext dataContext) : base(dataContext)
        {
        }

        public async Task<B2BCoursePromoPostDetailDto> GetDetailAsync(Guid postId, CancellationToken ct = default)
        {
            var post = await GetAsync(
                p => p.Id == postId,
                query => query.Include(p => p.CourseType),
                ct);

            var postEntity = post!;

            var author = await DataContext.Users
                .AsNoTracking()
                .Where(user => user.Id == postEntity.AuthorId)
                .Select(user => user.UserName)
                .FirstOrDefaultAsync(ct);

            InstructorProfileDto? instructorDto = null;
            if (postEntity.Instructor != null)
            {
                instructorDto = new InstructorProfileDto(
                    postEntity.Instructor.Name,
                    postEntity.Instructor.LinkedIn,
                    postEntity.Instructor.ReviewsLink);
            }

            return new B2BCoursePromoPostDetailDto(
                postEntity.Id,
                postEntity.AuthorId,
                author ?? string.Empty,
                postEntity.CourseName,
                postEntity.Title,
                postEntity.CourseTypeId,
                postEntity.CourseType.Name,
                postEntity.EventFormat,
                postEntity.TargetMajor,
                postEntity.StartAt,
                postEntity.EndAt,
                postEntity.LocationOrLink,
                postEntity.MaxAttendees,
                postEntity.DurationInfo,
                postEntity.IsPaid,
                postEntity.Price,
                postEntity.HasDiscount,
                postEntity.DiscountNote,
                postEntity.HasCertificate,
                postEntity.Content,
                postEntity.ApplicationMethod,
                postEntity.ApplicationLink,
                instructorDto,
                postEntity.IsEdited,
                postEntity.CreatedAt
            );
        }

        public async Task<PagedResult<B2BCoursePromoPostPagedItemDto>> GetPagedAsync(B2BCoursePromoPostPagedFilter filter, CancellationToken ct = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("CourseTypeId", filter.CourseTypeId);
            parameters.Add("EventFormat", filter.EventFormat?.ToString()); // Enum as string if saved as string, wait let me check how EventFormat is stored. Yes, in EF it's usually mapped but in Dapper it's raw. The enum EventFormat is mapped via .HasConversion<string>() in EF configuration, so in DB it's a string.
            parameters.Add("IsPaid", filter.IsPaid);
            parameters.Add("TargetMajor", string.IsNullOrWhiteSpace(filter.TargetMajor) ? null : filter.TargetMajor);
            parameters.Add("StartAtMin", filter.StartAtMin);
            parameters.Add("StartAtMax", filter.StartAtMax);

            var countSql = """
                SELECT COUNT(1)
                FROM Posts.Posts p
                INNER JOIN Posts.B2BCoursePromoPosts bcp ON bcp.Id = p.Id
                WHERE p.DeletedAt IS NULL
                  AND (@CourseTypeId IS NULL OR bcp.CourseTypeId = @CourseTypeId)
                  AND (@EventFormat IS NULL OR bcp.EventFormat = @EventFormat)
                  AND (@IsPaid IS NULL OR bcp.IsPaid = @IsPaid)
                  AND (@TargetMajor IS NULL OR bcp.TargetMajor LIKE '%' + @TargetMajor + '%')
                  AND (@StartAtMin IS NULL OR bcp.StartAt >= @StartAtMin)
                  AND (@StartAtMax IS NULL OR bcp.StartAt <= @StartAtMax);
                """;

            var dataSql = """
                SELECT p.Id,
                       p.AuthorId,
                       bcp.Title,
                       bcp.CourseName,
                       bcp.CourseTypeId,
                       ct.Name AS CourseTypeName,
                       bcp.EventFormat,
                       bcp.StartAt,
                       bcp.EndAt,
                       bcp.LocationOrLink,
                       bcp.MaxAttendees,
                       bcp.IsPaid,
                       bcp.Price,
                       bcp.HasDiscount,
                       p.CreatedAt
                FROM Posts.Posts p
                INNER JOIN Posts.B2BCoursePromoPosts bcp ON bcp.Id = p.Id
                LEFT JOIN Posts.CourseTypes ct ON ct.Id = bcp.CourseTypeId
                WHERE p.DeletedAt IS NULL
                  AND (@CourseTypeId IS NULL OR bcp.CourseTypeId = @CourseTypeId)
                  AND (@EventFormat IS NULL OR bcp.EventFormat = @EventFormat)
                  AND (@IsPaid IS NULL OR bcp.IsPaid = @IsPaid)
                  AND (@TargetMajor IS NULL OR bcp.TargetMajor LIKE '%' + @TargetMajor + '%')
                  AND (@StartAtMin IS NULL OR bcp.StartAt >= @StartAtMin)
                  AND (@StartAtMax IS NULL OR bcp.StartAt <= @StartAtMax)
                ORDER BY p.CreatedAt DESC, p.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                """;

            return await GetPagedInternalAsync<B2BCoursePromoPostPagedItemDto, B2BCoursePromoPostPagedRow>(
                filter.Page,
                filter.PageSize,
                parameters,
                countSql,
                dataSql,
                row => new B2BCoursePromoPostPagedItemDto
                {
                    Id = row.Id,
                    AuthorId = row.AuthorId,
                    Title = row.Title,
                    CourseName = row.CourseName,
                    CourseTypeId = row.CourseTypeId,
                    CourseTypeName = row.CourseTypeName,
                    EventFormat = Enum.Parse<Domain.Models.Enums.EventFormat>(row.EventFormat),
                    StartAt = row.StartAt,
                    EndAt = row.EndAt,
                    LocationOrLink = row.LocationOrLink,
                    MaxAttendees = row.MaxAttendees,
                    IsPaid = row.IsPaid,
                    Price = row.Price,
                    HasDiscount = row.HasDiscount,
                    CreatedAt = row.CreatedAt
                },
                ct);
        }

        private sealed class B2BCoursePromoPostPagedRow
        {
            public Guid Id { get; set; }
            public Guid AuthorId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string CourseName { get; set; } = string.Empty;
            public Guid CourseTypeId { get; set; }
            public string CourseTypeName { get; set; } = string.Empty;
            public string EventFormat { get; set; } = string.Empty;
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public string LocationOrLink { get; set; } = string.Empty;
            public int MaxAttendees { get; set; }
            public bool IsPaid { get; set; }
            public decimal? Price { get; set; }
            public bool HasDiscount { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

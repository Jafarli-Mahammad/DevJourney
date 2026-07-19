using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public record GetB2BCoursePromoPostsQuery(B2BCoursePromoPostPagedFilter Filter)
        : IRequest<PagedResult<B2BCoursePromoPostPagedItemDto>>;
}

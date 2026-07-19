using Application.Repositories;
using MediatR;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public record GetCorporateEventPostsQuery(CorporateEventPostPagedFilter Filter)
        : IRequest<PagedResult<CorporateEventPostPagedItemDto>>;
}

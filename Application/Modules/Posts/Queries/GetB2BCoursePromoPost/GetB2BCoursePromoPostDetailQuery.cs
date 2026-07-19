using MediatR;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public record GetB2BCoursePromoPostDetailQuery(Guid Id) : IRequest<B2BCoursePromoPostDetailDto>;
}

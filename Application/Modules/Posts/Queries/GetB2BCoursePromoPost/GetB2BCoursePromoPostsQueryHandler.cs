using Application.Repositories;
using Application.Repositories.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public class GetB2BCoursePromoPostsQueryHandler : IRequestHandler<GetB2BCoursePromoPostsQuery, PagedResult<B2BCoursePromoPostPagedItemDto>>
    {
        private readonly IB2BCoursePromoPostRepository _b2bCoursePromoPostRepository;

        public GetB2BCoursePromoPostsQueryHandler(IB2BCoursePromoPostRepository b2bCoursePromoPostRepository)
        {
            _b2bCoursePromoPostRepository = b2bCoursePromoPostRepository;
        }

        public Task<PagedResult<B2BCoursePromoPostPagedItemDto>> Handle(GetB2BCoursePromoPostsQuery request, CancellationToken cancellationToken)
        {
            return _b2bCoursePromoPostRepository.GetPagedAsync(request.Filter, cancellationToken);
        }
    }
}

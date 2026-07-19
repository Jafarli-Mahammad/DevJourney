using Application.Repositories.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public class GetB2BCoursePromoPostDetailQueryHandler : IRequestHandler<GetB2BCoursePromoPostDetailQuery, B2BCoursePromoPostDetailDto>
    {
        private readonly IB2BCoursePromoPostRepository _b2bCoursePromoPostRepository;

        public GetB2BCoursePromoPostDetailQueryHandler(IB2BCoursePromoPostRepository b2bCoursePromoPostRepository)
        {
            _b2bCoursePromoPostRepository = b2bCoursePromoPostRepository;
        }

        public Task<B2BCoursePromoPostDetailDto> Handle(GetB2BCoursePromoPostDetailQuery request, CancellationToken cancellationToken)
        {
            return _b2bCoursePromoPostRepository.GetDetailAsync(request.Id, cancellationToken);
        }
    }
}

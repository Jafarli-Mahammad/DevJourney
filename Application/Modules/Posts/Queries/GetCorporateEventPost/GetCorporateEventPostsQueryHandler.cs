using Application.Repositories;
using Application.Repositories.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public class GetCorporateEventPostsQueryHandler : IRequestHandler<GetCorporateEventPostsQuery, PagedResult<CorporateEventPostPagedItemDto>>
    {
        private readonly ICorporateEventPostRepository _corporateEventPostRepository;

        public GetCorporateEventPostsQueryHandler(ICorporateEventPostRepository corporateEventPostRepository)
        {
            _corporateEventPostRepository = corporateEventPostRepository;
        }

        public Task<PagedResult<CorporateEventPostPagedItemDto>> Handle(GetCorporateEventPostsQuery request, CancellationToken cancellationToken)
        {
            return _corporateEventPostRepository.GetPagedAsync(request.Filter, cancellationToken);
        }
    }
}

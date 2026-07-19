using Application.Repositories.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public class GetCorporateEventPostDetailQueryHandler : IRequestHandler<GetCorporateEventPostDetailQuery, CorporateEventPostDetailDto>
    {
        private readonly ICorporateEventPostRepository _corporateEventPostRepository;

        public GetCorporateEventPostDetailQueryHandler(ICorporateEventPostRepository corporateEventPostRepository)
        {
            _corporateEventPostRepository = corporateEventPostRepository;
        }

        public Task<CorporateEventPostDetailDto> Handle(GetCorporateEventPostDetailQuery request, CancellationToken cancellationToken)
        {
            return _corporateEventPostRepository.GetDetailAsync(request.Id, cancellationToken);
        }
    }
}

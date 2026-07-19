using MediatR;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public record GetCorporateEventPostDetailQuery(Guid Id) : IRequest<CorporateEventPostDetailDto>;
}

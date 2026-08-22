using Application.Repositories;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.PartnerAccounts.Queries.GetPartnerAccounts
{
    public class GetPartnerAccountsQuery : IRequest<object>
    {
    }

    public class GetPartnerAccountsQueryHandler : IRequestHandler<GetPartnerAccountsQuery, object>
    {
        private readonly IPartnerProfileRepository _partnerProfileRepository;

        public GetPartnerAccountsQueryHandler(IPartnerProfileRepository partnerProfileRepository)
        {
            _partnerProfileRepository = partnerProfileRepository;
        }

        public async Task<object> Handle(GetPartnerAccountsQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _partnerProfileRepository.GetAllAsync(null, cancellationToken);
            return accounts.Select(a => new
            {
                a.Id,
                a.PartnerName,
                a.RepresentativeName,
                a.ContactEmail
            }).ToList();
        }
    }
}

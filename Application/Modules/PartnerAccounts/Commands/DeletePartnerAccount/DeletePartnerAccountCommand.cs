using Application.Exceptions;
using Application.Repositories;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.PartnerAccounts.Commands.DeletePartnerAccount
{
    public class DeletePartnerAccountCommand : IRequest<bool>
    {
        public Guid AccountId { get; set; }
    }

    public class DeletePartnerAccountCommandHandler : IRequestHandler<DeletePartnerAccountCommand, bool>
    {
        private readonly IPartnerProfileRepository _partnerProfileRepository;

        public DeletePartnerAccountCommandHandler(IPartnerProfileRepository partnerProfileRepository)
        {
            _partnerProfileRepository = partnerProfileRepository;
        }

        public async Task<bool> Handle(DeletePartnerAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _partnerProfileRepository.GetAsync(a => a.Id == request.AccountId, null, cancellationToken);
            if (account == null) throw new NotFoundException("PartnerAccount", request.AccountId);
            
            _partnerProfileRepository.Remove(account);
            return true;
        }
    }
}

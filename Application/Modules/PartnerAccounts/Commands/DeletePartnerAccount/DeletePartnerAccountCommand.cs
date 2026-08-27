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
        private readonly IJuryProfileRepository _juryProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePartnerAccountCommandHandler(
            IJuryProfileRepository juryProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _juryProfileRepository = juryProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePartnerAccountCommand request, CancellationToken cancellationToken)
        {
            var juries = await _juryProfileRepository.GetAllAsync(
                j => j.Id == request.AccountId || j.ApplicationUserId == request.AccountId,
                cancellationToken);
            var jury = juries.FirstOrDefault();
            
            if (jury != null)
            {
                _juryProfileRepository.Remove(jury);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}

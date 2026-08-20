using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Models.Entities.Company;
using Domain.Models.Enums;
using Application.Repositories.Company;
using Application.Repositories;

namespace Application.Modules.PartnerAuth.Commands.GenerateInvitation
{
    public class GenerateMockInvitationCommand : IRequest<GenerateMockInvitationDto>
    {
        public string CompanyName { get; set; } = string.Empty;
        public PartnerType PartnerType { get; set; }
    }

    public class GenerateMockInvitationDto
    {
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class GenerateMockInvitationCommandHandler : IRequestHandler<GenerateMockInvitationCommand, GenerateMockInvitationDto>
    {
        private readonly ICompanyInvitationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public GenerateMockInvitationCommandHandler(ICompanyInvitationRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<GenerateMockInvitationDto> Handle(GenerateMockInvitationCommand request, CancellationToken cancellationToken)
        {
            var code = $"INVITE-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            var invite = new CompanyInvitation
            {
                Code = code,
                CompanyName = request.CompanyName,
                PartnerType = request.PartnerType,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsUsed = false
            };

            await _repository.AddAsync(invite, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new GenerateMockInvitationDto
            {
                Code = invite.Code,
                ExpiresAt = invite.ExpiresAt
            };
        }
    }
}

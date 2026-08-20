using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Models.Enums;
using Application.Exceptions;
using Application.Repositories.Company;
using System.Linq;

namespace Application.Modules.PartnerAuth.Queries.VerifyInvitation
{
    public class VerifyInvitationQuery : IRequest<VerifyInvitationDto>
    {
        public string Code { get; set; } = string.Empty;
    }

    public class VerifyInvitationDto
    {
        public string Code { get; set; } = string.Empty;
        public string PartnerType { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsValid { get; set; }
    }

    public class VerifyInvitationQueryHandler : IRequestHandler<VerifyInvitationQuery, VerifyInvitationDto>
    {
        private readonly ICompanyInvitationRepository _repository;

        public VerifyInvitationQueryHandler(ICompanyInvitationRepository repository)
        {
            _repository = repository;
        }

        public async Task<VerifyInvitationDto> Handle(VerifyInvitationQuery request, CancellationToken cancellationToken)
        {
            var invites = await _repository.GetAllAsync(i => i.Code == request.Code, cancellationToken);
            var invite = invites.FirstOrDefault();

            if (invite == null)
                throw new NotFoundException("CompanyInvitation", request.Code);

            if (invite.IsUsed || invite.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("This invitation code has already been used or has expired.");

            return new VerifyInvitationDto
            {
                Code = invite.Code,
                PartnerType = invite.PartnerType.ToString(),
                OrganizationName = invite.CompanyName,
                ExpiresAt = invite.ExpiresAt,
                IsValid = true
            };
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Models.Enums;
using Domain.Models.Entities.Partner;
using Application.Exceptions;
using Application.Services;
using Application.Repositories;
using Application.Repositories.Company;
using System.Linq;

namespace Application.Modules.PartnerAuth.Commands.RegisterPartner
{
    public class RegisterPartnerCommand : IRequest<RegisterPartnerDto>
    {
        public string ConfirmCode { get; set; } = string.Empty;
        public PartnerType PartnerType { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RepresentativeName { get; set; } = string.Empty;
        public string RepresentativeRole { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterPartnerDto
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string PartnerType { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RepresentativeName { get; set; } = string.Empty;
        public string VerificationStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RegisterPartnerCommandHandler : IRequestHandler<RegisterPartnerCommand, RegisterPartnerDto>
    {
        private readonly ICompanyInvitationRepository _invitationRepository;
        private readonly IPartnerProfileRepository _partnerProfileRepository;
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterPartnerCommandHandler(
            ICompanyInvitationRepository invitationRepository,
            IPartnerProfileRepository partnerProfileRepository,
            IAuthService authService,
            IUnitOfWork unitOfWork)
        {
            _invitationRepository = invitationRepository;
            _partnerProfileRepository = partnerProfileRepository;
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterPartnerDto> Handle(RegisterPartnerCommand request, CancellationToken cancellationToken)
        {
            var invites = await _invitationRepository.GetAllAsync(i => i.Code == request.ConfirmCode, cancellationToken);
            var invite = invites.FirstOrDefault();

            if (invite == null)
                throw new NotFoundException("CompanyInvitation", request.ConfirmCode);

            if (invite.IsUsed || invite.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("This invitation code has already been used or has expired.");

            var userId = await _authService.RegisterAsync(request.Email, request.Email, request.Password);

            var partnerProfile = new PartnerProfile
            {
                ApplicationUserId = userId,
                PartnerName = request.OrganizationName,
                PartnerType = request.PartnerType,
                WebsiteUrl = request.WebsiteUrl,
                RepresentativeName = request.RepresentativeName,
                RepresentativeRole = request.RepresentativeRole,
                ContactEmail = request.Email,
                IsVerified = false // Needs SuperAdmin approval
            };

            await _partnerProfileRepository.AddAsync(partnerProfile, cancellationToken);

            invite.IsUsed = true;
            await _invitationRepository.EditAsync(invite);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterPartnerDto
            {
                Id = partnerProfile.Id,
                OrganizationName = partnerProfile.PartnerName,
                PartnerType = partnerProfile.PartnerType.ToString(),
                Email = request.Email,
                RepresentativeName = partnerProfile.RepresentativeName ?? "",
                VerificationStatus = "PENDING_ADMIN_REVIEW",
                CreatedAt = partnerProfile.CreatedAt
            };
        }
    }
}

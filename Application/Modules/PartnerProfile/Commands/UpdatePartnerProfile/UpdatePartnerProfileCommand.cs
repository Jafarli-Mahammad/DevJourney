using Application.Repositories;

using MediatR;
using Application.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;

namespace Application.Modules.PartnerProfile.Commands.UpdatePartnerProfile
{
    public class UpdatePartnerProfileCommand : IRequest<object>
    {
        public string? PartnerName { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? RepresentativeName { get; set; }
        public string? RepresentativeRole { get; set; }
        public string? ContactEmail { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
    }

    public class UpdatePartnerProfileCommandHandler : IRequestHandler<UpdatePartnerProfileCommand, object>
    {
        private readonly IPartnerProfileRepository _partnerProfileRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePartnerProfileCommandHandler(IPartnerProfileRepository partnerProfileRepository, ICurrentUserService currentUserService)
        {
            _partnerProfileRepository = partnerProfileRepository;
            _currentUserService = currentUserService;
        }

        public async Task<object> Handle(UpdatePartnerProfileCommand request, CancellationToken cancellationToken)
        {
            var profiles = await _partnerProfileRepository.GetAllAsync(p => p.ApplicationUserId == _currentUserService.UserId, cancellationToken);
            var profile = profiles.FirstOrDefault();

            if (profile == null)
                throw new NotFoundException("PartnerProfile", _currentUserService.UserId);

            if (request.PartnerName != null) profile.PartnerName = request.PartnerName;
            if (request.WebsiteUrl != null) profile.WebsiteUrl = request.WebsiteUrl;
            if (request.Location != null) profile.Location = request.Location;
            if (request.Description != null) profile.Description = request.Description;
            if (request.RepresentativeName != null) profile.RepresentativeName = request.RepresentativeName;
            if (request.RepresentativeRole != null) profile.RepresentativeRole = request.RepresentativeRole;
            if (request.ContactEmail != null) profile.ContactEmail = request.ContactEmail;
            if (request.LogoUrl != null) profile.LogoUrl = request.LogoUrl;
            if (request.BannerUrl != null) profile.BannerUrl = request.BannerUrl;

            await _partnerProfileRepository.EditAsync(profile);

            return new { profile.Id };
        }
    }
}

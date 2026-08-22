using Application.Repositories;

using MediatR;
using Application.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;

namespace Application.Modules.PartnerProfile.Queries.GetPartnerProfile
{
    public class GetPartnerProfileQuery : IRequest<object>
    {
    }

    public class GetPartnerProfileQueryHandler : IRequestHandler<GetPartnerProfileQuery, object>
    {
        private readonly IPartnerProfileRepository _partnerProfileRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetPartnerProfileQueryHandler(IPartnerProfileRepository partnerProfileRepository, ICurrentUserService currentUserService)
        {
            _partnerProfileRepository = partnerProfileRepository;
            _currentUserService = currentUserService;
        }

        public async Task<object> Handle(GetPartnerProfileQuery request, CancellationToken cancellationToken)
        {
            var profiles = await _partnerProfileRepository.GetAllAsync(p => p.ApplicationUserId == _currentUserService.UserId, cancellationToken);
            var profile = profiles.FirstOrDefault();

            if (profile == null)
                throw new NotFoundException("PartnerProfile", _currentUserService.UserId);

            return new
            {
                profile.Id,
                profile.PartnerName,
                PartnerType = profile.PartnerType.ToString(),
                profile.WebsiteUrl,
                profile.Location,
                profile.Description,
                profile.IsVerified,
                profile.RepresentativeName,
                profile.RepresentativeRole,
                profile.ContactEmail,
                profile.LogoUrl,
                profile.BannerUrl
            };
        }
    }
}

using Application.Modules.Profile.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Profile.Queries.GetMyProfile
{
    public class GetMyProfileQuery : IRequest<ProfileDto>
    {
    }

    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileDto>
    {
        private readonly Application.Repositories.IStudentProfileRepository _studentProfileRepository;
        private readonly Application.Services.ICurrentUserService _currentUserService;

        public GetMyProfileQueryHandler(
            Application.Repositories.IStudentProfileRepository studentProfileRepository,
            Application.Services.ICurrentUserService currentUserService)
        {
            _studentProfileRepository = studentProfileRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                return new ProfileDto { Id = Guid.Empty, FullName = "Unknown", Bio = null, AvatarUrl = null };
            }

            var profile = await _studentProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return new ProfileDto { Id = userId, FullName = "New User", Bio = null, AvatarUrl = null };
            }

            return new ProfileDto 
            { 
                Id = profile.Id, // Frontend probably expects the Profile ID, not the ApplicationUser ID? Wait.
                FullName = $"{profile.FirstName} {profile.LastName}".Trim(), 
                Bio = profile.Bio, 
                AvatarUrl = null // Update if avatar exists
            };
        }
    }
}

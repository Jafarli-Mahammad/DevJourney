using Application.Modules.Profile.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Profile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<ProfileDto>
    {
        public string FullName { get; set; } = null!;
        public string? Bio { get; set; }
    }

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto>
    {
        private readonly Application.Services.ICurrentUserService _currentUserService;
        private readonly Application.Repositories.IStudentProfileRepository _studentProfileRepository;
        private readonly Application.Repositories.IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(
            Application.Services.ICurrentUserService currentUserService,
            Application.Repositories.IStudentProfileRepository studentProfileRepository,
            Application.Repositories.IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _studentProfileRepository = studentProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == System.Guid.Empty)
            {
                throw new Application.Exceptions.UnauthorizedException();
            }

            var profile = await _studentProfileRepository.GetByUserIdAsync(userId);
            if (profile != null)
            {
                var nameParts = (request.FullName ?? "").Trim().Split(' ', 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (nameParts.Length > 0)
                {
                    profile.FirstName = nameParts[0];
                    profile.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                }
                profile.Bio = request.Bio;

                await _studentProfileRepository.EditAsync(profile);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ProfileDto
                {
                    Id = profile.Id,
                    FullName = $"{profile.FirstName} {profile.LastName}".Trim(),
                    Bio = profile.Bio,
                    AvatarUrl = null
                };
            }

            return new ProfileDto
            {
                Id = userId,
                FullName = request.FullName,
                Bio = request.Bio,
                AvatarUrl = null
            };
        }
    }
}

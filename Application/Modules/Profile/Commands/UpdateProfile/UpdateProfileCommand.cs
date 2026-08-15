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
        public Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ProfileDto { Id = System.Guid.NewGuid(), FullName = request.FullName, Bio = request.Bio });
        }
    }
}

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
        public Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ProfileDto { Id = Guid.NewGuid(), FullName = "Mock User", Bio = "Mock Bio", AvatarUrl = null });
        }
    }
}

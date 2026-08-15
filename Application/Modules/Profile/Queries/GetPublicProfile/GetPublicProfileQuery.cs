using Application.Modules.Profile.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Profile.Queries.GetPublicProfile
{
    public class GetPublicProfileQuery : IRequest<PublicProfileDto>
    {
        public string IdOrSlug { get; set; } = null!;
    }

    public class GetPublicProfileQueryHandler : IRequestHandler<GetPublicProfileQuery, PublicProfileDto>
    {
        public Task<PublicProfileDto> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new PublicProfileDto { Id = Guid.NewGuid(), FullName = "Public Mock User", Bio = "Public Bio", AvatarUrl = null });
        }
    }
}

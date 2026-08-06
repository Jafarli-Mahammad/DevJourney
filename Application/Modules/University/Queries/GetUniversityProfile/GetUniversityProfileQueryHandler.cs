using Application.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.University.Queries.GetUniversityProfile
{
    public class GetUniversityProfileQueryHandler : IRequestHandler<GetUniversityProfileQuery, UniversityProfileDto>
    {
        private readonly IUniversityProfileRepository _universityProfileRepository;

        public GetUniversityProfileQueryHandler(IUniversityProfileRepository universityProfileRepository)
        {
            _universityProfileRepository = universityProfileRepository;
        }

        public async Task<UniversityProfileDto> Handle(GetUniversityProfileQuery request, CancellationToken cancellationToken)
        {
            var entity = await _universityProfileRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return null!;
            }

            return new UniversityProfileDto
            {
                Id = entity.Id,
                UniversityName = entity.UniversityName,
                WebsiteUrl = entity.WebsiteUrl,
                Location = entity.Location,
                RepresentativeName = entity.RepresentativeName,
                RepresentativeEmail = entity.RepresentativeEmail,
                IsVerified = entity.IsVerified
            };
        }
    }
}

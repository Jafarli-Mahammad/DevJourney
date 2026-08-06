using Application.Modules.University.Queries.GetUniversityProfile;
using Application.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.University.Queries.GetAllUniversityProfiles
{
    public class GetAllUniversityProfilesQueryHandler : IRequestHandler<GetAllUniversityProfilesQuery, List<UniversityProfileDto>>
    {
        private readonly IUniversityProfileRepository _universityProfileRepository;

        public GetAllUniversityProfilesQueryHandler(IUniversityProfileRepository universityProfileRepository)
        {
            _universityProfileRepository = universityProfileRepository;
        }

        public async Task<List<UniversityProfileDto>> Handle(GetAllUniversityProfilesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _universityProfileRepository.GetAllAsync(cancellationToken: cancellationToken);

            return entities.Select(entity => new UniversityProfileDto
            {
                Id = entity.Id,
                UniversityName = entity.UniversityName,
                WebsiteUrl = entity.WebsiteUrl,
                Location = entity.Location,
                RepresentativeName = entity.RepresentativeName,
                RepresentativeEmail = entity.RepresentativeEmail,
                IsVerified = entity.IsVerified
            }).ToList();
        }
    }
}

using Application.Modules.Jury.Queries.GetJuryProfile;
using Application.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Queries.GetAllJuryProfiles
{
    public class GetAllJuryProfilesQueryHandler : IRequestHandler<GetAllJuryProfilesQuery, List<JuryProfileDto>>
    {
        private readonly IJuryProfileRepository _juryProfileRepository;

        public GetAllJuryProfilesQueryHandler(IJuryProfileRepository juryProfileRepository)
        {
            _juryProfileRepository = juryProfileRepository;
        }

        public async Task<List<JuryProfileDto>> Handle(GetAllJuryProfilesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _juryProfileRepository.GetAllAsync(cancellationToken: cancellationToken);

            return entities.Select(entity => new JuryProfileDto
            {
                Id = entity.Id,
                JuryCode = entity.JuryCode,
                FullName = entity.FullName,
                Email = entity.Email,
                Specialization = entity.Specialization,
                CompetitionId = entity.CompetitionId
            }).ToList();
        }
    }
}

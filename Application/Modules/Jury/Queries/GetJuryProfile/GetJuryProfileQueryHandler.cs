using Application.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Queries.GetJuryProfile
{
    public class GetJuryProfileQueryHandler : IRequestHandler<GetJuryProfileQuery, JuryProfileDto>
    {
        private readonly IJuryProfileRepository _juryProfileRepository;

        public GetJuryProfileQueryHandler(IJuryProfileRepository juryProfileRepository)
        {
            _juryProfileRepository = juryProfileRepository;
        }

        public async Task<JuryProfileDto> Handle(GetJuryProfileQuery request, CancellationToken cancellationToken)
        {
            var entity = await _juryProfileRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return null!;
            }

            return new JuryProfileDto
            {
                Id = entity.Id,
                JuryCode = entity.JuryCode,
                FullName = entity.FullName,
                Email = entity.Email,
                Specialization = entity.Specialization,
                CompetitionId = entity.CompetitionId
            };
        }
    }
}

using Application.Repositories;
using MediatR;

namespace Application.Modules.Professions.Queries.GetAll
{
    public class GetAllProfessionsQueryHandler : IRequestHandler<GetAllProfessionsQuery, List<ProfessionDto>>
    {
        private readonly IProfessionRepository _professionRepository;

        public GetAllProfessionsQueryHandler(IProfessionRepository professionRepository)
        {
            _professionRepository = professionRepository;
        }

        public async Task<List<ProfessionDto>> Handle(GetAllProfessionsQuery request, CancellationToken cancellationToken)
        {
            var data = await _professionRepository.GetAllAsync(cancellationToken: cancellationToken);
            return data.Select(p => new ProfessionDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }
    }
}

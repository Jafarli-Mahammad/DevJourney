using Microsoft.EntityFrameworkCore;
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Modules.Skills.Queries.GetAll
{
    public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, List<SkillDto>>
    {
        private readonly ISkillRepository skillRepository;
        private readonly IMapper mapper;

        public GetAllSkillsQueryHandler(ISkillRepository skillRepository, IMapper mapper)
        {
            this.skillRepository = skillRepository;
            this.mapper = mapper;
        }

        public async Task<List<SkillDto>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
        {
            var result = await skillRepository.GetAllAsync(cancellationToken: cancellationToken);
            return mapper.Map<List<SkillDto>>(result);
        }
    }
}

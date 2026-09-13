using Microsoft.EntityFrameworkCore;
using Application.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Skills.Queries.GetAll
{
    public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, List<SkillDto>>
    {
        private readonly ISkillRepository skillRepository;
        private readonly IMapper mapper;
        private readonly HybridCache hybridCache;

        public GetAllSkillsQueryHandler(ISkillRepository skillRepository, IMapper mapper, HybridCache hybridCache)
        {
            this.skillRepository = skillRepository;
            this.mapper = mapper;
            this.hybridCache = hybridCache;
        }

        public async Task<List<SkillDto>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
        {
            return await hybridCache.GetOrCreateAsync(
                "lookups:skills",
                async token =>
                {
                    var result = await skillRepository.GetAllAsync(cancellationToken: token);
                    return mapper.Map<List<SkillDto>>(result);
                },
                tags: new[] { "lookups" },
                cancellationToken: cancellationToken);
        }
    }
}

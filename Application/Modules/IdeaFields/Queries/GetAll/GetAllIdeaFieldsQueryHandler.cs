using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Modules.IdeaFields.Queries.GetAll
{
    public class GetAllIdeaFieldsQueryHandler : IRequestHandler<GetAllIdeaFieldsQuery, List<IdeaFieldDto>>
    {
        private readonly IIdeaFieldRepository ideaFieldRepository;
        private readonly IMapper mapper;

        public GetAllIdeaFieldsQueryHandler(IIdeaFieldRepository ideaFieldRepository, IMapper mapper)
        {
            this.ideaFieldRepository = ideaFieldRepository;
            this.mapper = mapper;
        }

        public async Task<List<IdeaFieldDto>> Handle(GetAllIdeaFieldsQuery request, CancellationToken cancellationToken)
        {
            var result = await ideaFieldRepository.GetAllAsync(cancellationToken: cancellationToken);
            return mapper.Map<List<IdeaFieldDto>>(result);
        }
    }
}
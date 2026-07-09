using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Modules.Languages.Queries.GetAll
{
    public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesQuery, List<LanguageDto>>
    {
        private readonly ILanguageRepository languageRepository;
        private readonly IMapper mapper;

        public GetAllLanguagesQueryHandler(ILanguageRepository languageRepository, IMapper mapper)
        {
            this.languageRepository = languageRepository;
            this.mapper = mapper;
        }

        public async Task<List<LanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            var result = await languageRepository.GetAllAsync(cancellationToken: cancellationToken);
            return mapper.Map<List<LanguageDto>>(result);
        }
    }
}

using MediatR;

namespace Application.Modules.Languages.Queries.GetAll
{
    public class GetAllLanguagesQuery : IRequest<List<LanguageDto>>
    {
    }
}

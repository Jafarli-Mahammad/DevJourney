using MediatR;

namespace Application.Modules.IdeaFields.Queries.GetAll
{
    public class GetAllIdeaFieldsQuery : IRequest<List<IdeaFieldDto>>
    {
    }
}
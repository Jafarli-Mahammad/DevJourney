using MediatR;

namespace Application.Modules.Skills.Queries.GetAll
{
    public class GetAllSkillsQuery : IRequest<List<SkillDto>>
    {
    }
}

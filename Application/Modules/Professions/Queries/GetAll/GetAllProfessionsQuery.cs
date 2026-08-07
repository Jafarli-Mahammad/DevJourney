using MediatR;

namespace Application.Modules.Professions.Queries.GetAll
{
    public record GetAllProfessionsQuery : IRequest<List<ProfessionDto>>;
}

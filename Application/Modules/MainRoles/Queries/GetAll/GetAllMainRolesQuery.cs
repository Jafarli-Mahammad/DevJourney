using MediatR;

namespace Application.Modules.MainRoles.Queries.GetAll
{
    public record GetAllMainRolesQuery : IRequest<List<MainRoleDto>>;
}

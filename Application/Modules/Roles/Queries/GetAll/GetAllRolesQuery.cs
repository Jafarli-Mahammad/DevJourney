using MediatR;

namespace Application.Modules.Roles.Queries.GetAll
{
    public class GetAllRolesQuery : IRequest<List<RoleDto>>
    {
    }
}

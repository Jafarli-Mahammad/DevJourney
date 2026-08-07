using Application.Repositories;
using MediatR;

namespace Application.Modules.MainRoles.Queries.GetAll
{
    public class GetAllMainRolesQueryHandler : IRequestHandler<GetAllMainRolesQuery, List<MainRoleDto>>
    {
        private readonly IMainRoleRepository _mainRoleRepository;

        public GetAllMainRolesQueryHandler(IMainRoleRepository mainRoleRepository)
        {
            _mainRoleRepository = mainRoleRepository;
        }

        public async Task<List<MainRoleDto>> Handle(GetAllMainRolesQuery request, CancellationToken cancellationToken)
        {
            var data = await _mainRoleRepository.GetAllAsync(cancellationToken: cancellationToken);
            return data.Select(r => new MainRoleDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }
    }
}

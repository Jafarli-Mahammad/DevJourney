using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Modules.Roles.Queries.GetAll
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;

        public GetAllRolesQueryHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
        }

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await roleRepository.GetAllAsync(cancellationToken: cancellationToken);
            return mapper.Map<List<RoleDto>>(result);
        }
    }
}

using Application.Modules.Profile.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Profile.Queries.GetMe
{
    public class GetMeQuery : IRequest<MeDto>
    {
    }

    public class GetMeQueryHandler : IRequestHandler<GetMeQuery, MeDto>
    {
        private readonly Application.Services.ICurrentUserService _currentUserService;
        private readonly Application.Repositories.IStudentProfileRepository _studentProfileRepository;
        private readonly Application.Repositories.IPartnerProfileRepository _partnerProfileRepository;

        public GetMeQueryHandler(
            Application.Services.ICurrentUserService currentUserService,
            Application.Repositories.IStudentProfileRepository studentProfileRepository,
            Application.Repositories.IPartnerProfileRepository partnerProfileRepository)
        {
            _currentUserService = currentUserService;
            _studentProfileRepository = studentProfileRepository;
            _partnerProfileRepository = partnerProfileRepository;
        }

        public async Task<MeDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            string role = "Student";
            
            var isPartner = await _partnerProfileRepository.GetAllAsync(p => p.ApplicationUserId == _currentUserService.UserId, cancellationToken);
            if (System.Linq.Enumerable.Any(isPartner))
            {
                role = "Partner";
            }
            
            return new MeDto 
            { 
                Id = _currentUserService.UserId, 
                Email = _currentUserService.Email ?? "", 
                Role = role
            };
        }
    }
}

using Application.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Student.Queries.GetAllStudentProfiles
{
    public class GetAllStudentProfilesQueryHandler : IRequestHandler<GetAllStudentProfilesQuery, List<StudentProfileListDto>>
    {
        private readonly IStudentProfileRepository _studentProfileRepository;

        public GetAllStudentProfilesQueryHandler(IStudentProfileRepository studentProfileRepository)
        {
            _studentProfileRepository = studentProfileRepository;
        }

        public async Task<List<StudentProfileListDto>> Handle(GetAllStudentProfilesQuery request, CancellationToken cancellationToken)
        {
            var data = await _studentProfileRepository.GetAllWithEmailAsync(cancellationToken);

            return data.Select(item => new StudentProfileListDto
            {
                Id = item.Profile.Id,
                ApplicationUserId = item.Profile.ApplicationUserId,
                Email = item.Email,
                UniversityId = item.Profile.UniversityId,
                Bio = item.Profile.Bio,
                ProfessionalRole = item.Profile.ProfessionalRole,
                GitHubUrl = item.Profile.GitHubUrl,
                LinkedinUrl = item.Profile.LinkedinUrl
            }).ToList();
        }
    }
}

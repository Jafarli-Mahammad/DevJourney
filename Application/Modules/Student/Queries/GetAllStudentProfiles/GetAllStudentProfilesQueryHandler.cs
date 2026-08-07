using Application.Repositories;
using MediatR;

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
                FirstName = item.Profile.FirstName,
                LastName = item.Profile.LastName,
                Email = item.Email,
                UniversityId = item.Profile.UniversityId,
                UniversityName = item.Profile.University?.UniversityName,
                ProfessionId = item.Profile.ProfessionId,
                ProfessionName = item.Profile.Profession?.Name,
                MainRoleId = item.Profile.MainRoleId,
                MainRoleName = item.Profile.MainRole?.Name,
                Bio = item.Profile.Bio,
                GitHubUrl = item.Profile.GitHubUrl,
                LinkedinUrl = item.Profile.LinkedinUrl,
                CompletionPercentage = item.Profile.CalculateProfileCompletionPercentage()
            }).ToList();
        }
    }
}

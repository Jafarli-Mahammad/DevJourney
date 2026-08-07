using Application.Modules.Skills;
using Application.Repositories;
using MediatR;

namespace Application.Modules.Student.Queries.GetStudentProfile
{
    public class GetStudentProfileQueryHandler : IRequestHandler<GetStudentProfileQuery, StudentProfileDto>
    {
        private readonly IStudentProfileRepository _studentProfileRepository;

        public GetStudentProfileQueryHandler(IStudentProfileRepository studentProfileRepository)
        {
            _studentProfileRepository = studentProfileRepository;
        }

        public async Task<StudentProfileDto> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
        {
            var data = await _studentProfileRepository.GetWithEmailByIdAsync(request.Id, cancellationToken);
            if (data == null)
            {
                return null!;
            }

            var profile = data.Value.Profile;
            var email = data.Value.Email;

            return new StudentProfileDto
            {
                Id = profile.Id,
                ApplicationUserId = profile.ApplicationUserId,
                Email = email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                UniversityId = profile.UniversityId,
                UniversityName = profile.University?.UniversityName,
                PhoneNumber = profile.PhoneNumber,
                ProfessionId = profile.ProfessionId,
                ProfessionName = profile.Profession?.Name,
                Course = profile.Course,
                GitHubUrl = profile.GitHubUrl,
                LinkedinUrl = profile.LinkedinUrl,
                PortfolioUrl = profile.PortfolioUrl,
                CVUrl = profile.CVUrl,
                MainRoleId = profile.MainRoleId,
                MainRoleName = profile.MainRole?.Name,
                ExperienceLevel = profile.ExperienceLevel,
                Bio = profile.Bio,
                CompletionPercentage = profile.CalculateProfileCompletionPercentage(),
                Skills = profile.StudentSkills.Select(ss => new SkillDto
                {
                    Id = ss.SkillId,
                    Name = ss.Skill?.Name ?? string.Empty
                }).ToList(),
                Languages = profile.StudentLanguages.Select(sl => new StudentLanguageDto
                {
                    LanguageId = sl.LanguageId,
                    LanguageName = sl.Language?.Name,
                    ProficiencyLevel = sl.ProficiencyLevel
                }).ToList()
            };
        }
    }
}

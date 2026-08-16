using Application.Exceptions;
using Application.Modules.Student.Queries.GetStudentProfile;
using Application.Repositories;
using Application.Services;
using Domain.Models.Entities.Student;
using MediatR;

namespace Application.Modules.Student.Commands.UpdateCabinetProfile
{
    public class UpdateStudentProfileCommandHandler : IRequestHandler<UpdateStudentProfileCommand, StudentProfileDto>
    {
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentProfileCommandHandler(
            IStudentProfileRepository studentProfileRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _studentProfileRepository = studentProfileRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentProfileDto> Handle(UpdateStudentProfileCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                throw new UnauthorizedException();
            }

            // SEC: Prevent IDOR by loading resource directly via authenticated user context
            var profile = await _studentProfileRepository.GetFullProfileByUserIdAsync(_currentUserService.UserId, cancellationToken);

            if (profile == null)
            {
                throw new NotFoundException("StudentProfile", _currentUserService.UserId);
            }

            profile.UpdateCabinetProfile(
                request.UniversityId,
                request.PhoneNumber,
                request.ProfessionId,
                request.Course,
                request.GitHubUrl,
                request.LinkedinUrl,
                request.PortfolioUrl,
                request.CVUrl,
                request.MainRoleId,
                request.ExperienceLevel,
                request.Bio);

            profile.SetSkills(request.SkillIds);

            if (request.Languages != null)
            {
                var langTuples = request.Languages
                    .Select(l => (l.LanguageId, l.ProficiencyLevel));
                profile.SetLanguages(langTuples);
            }
            else
            {
                profile.SetLanguages(null);
            }

            await _studentProfileRepository.EditAsync(profile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedData = await _studentProfileRepository.GetWithEmailByIdAsync(profile.Id, cancellationToken);

            var fullProfile = updatedData!.Value.Profile;
            var email = updatedData.Value.Email;

            return new StudentProfileDto
            {
                Id = fullProfile.Id,
                ApplicationUserId = fullProfile.ApplicationUserId,
                Email = email,
                FirstName = fullProfile.FirstName,
                LastName = fullProfile.LastName,
                UniversityId = fullProfile.UniversityId,
                UniversityName = fullProfile.University?.UniversityName,
                PhoneNumber = fullProfile.PhoneNumber,
                ProfessionId = fullProfile.ProfessionId,
                ProfessionName = fullProfile.Profession?.Name,
                Course = fullProfile.Course,
                GitHubUrl = fullProfile.GitHubUrl,
                LinkedinUrl = fullProfile.LinkedinUrl,
                PortfolioUrl = fullProfile.PortfolioUrl,
                CVUrl = fullProfile.CVUrl,
                MainRoleId = fullProfile.MainRoleId,
                MainRoleName = fullProfile.MainRole?.Name,
                ExperienceLevel = fullProfile.ExperienceLevel,
                Bio = fullProfile.Bio,
                CompletionPercentage = fullProfile.CalculateProfileCompletionPercentage(),
                Skills = fullProfile.StudentSkills.Select(ss => new Skills.SkillDto
                {
                    Id = ss.SkillId,
                    Name = ss.Skill?.Name ?? string.Empty
                }).ToList(),
                Languages = fullProfile.StudentLanguages.Select(sl => new StudentLanguageDto
                {
                    LanguageId = sl.LanguageId,
                    LanguageName = sl.Language?.Name,
                    ProficiencyLevel = sl.ProficiencyLevel
                }).ToList()
            };
        }
    }
}

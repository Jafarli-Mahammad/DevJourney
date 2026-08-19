using Application.Exceptions;
using Application.Modules.Skills;
using Application.Modules.Student.Queries.GetStudentProfile;
using Application.Repositories;
using Application.Repositories.Competitions;
using Application.Repositories.Core;
using Application.Services;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Student.Queries.GetMyStudentProfile
{
    public class GetMyStudentProfileQueryHandler : IRequestHandler<GetMyStudentProfileQuery, StudentProfileDto?>
    {
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICompetitionParticipantRepository _competitionParticipantRepository;
        private readonly ICompetitionRepository _competitionRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyStudentProfileQueryHandler(
            IStudentProfileRepository studentProfileRepository,
            ICertificateRepository certificateRepository,
            ICompetitionParticipantRepository competitionParticipantRepository,
            ICompetitionRepository competitionRepository,
            ICurrentUserService currentUserService)
        {
            _studentProfileRepository = studentProfileRepository;
            _certificateRepository = certificateRepository;
            _competitionParticipantRepository = competitionParticipantRepository;
            _competitionRepository = competitionRepository;
            _currentUserService = currentUserService;
        }

        public async Task<StudentProfileDto?> Handle(GetMyStudentProfileQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == System.Guid.Empty)
            {
                throw new UnauthorizedException();
            }

            var profile = await _studentProfileRepository.GetFullProfileByUserIdAsync(_currentUserService.UserId, cancellationToken);
            if (profile == null)
            {
                return null;
            }

            var email = _currentUserService.Email ?? string.Empty;

            // Fetch certificates
            var certificates = await _certificateRepository.GetAllAsync(c => c.UserId == profile.ApplicationUserId, cancellationToken);

            // Fetch competitions where student is a captain or a member
            var teams = await _competitionParticipantRepository.GetAllAsync(
                p => p.CaptainId == profile.Id || p.Members.Any(m => m.StudentProfileId == profile.Id),
                cancellationToken);

            // Fetch competition names
            var compIds = teams.Select(t => t.CompetitionId).Distinct().ToList();
            var competitions = await _competitionRepository.GetAllAsync(c => compIds.Contains(c.Id), cancellationToken);
            var compNames = competitions.ToDictionary(c => c.Id, c => c.Title);

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
                }).ToList(),
                Certificates = certificates.Select(c => new CertificateProfileDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    AssetId = c.AssetId
                }).ToList(),
                Competitions = teams.Select(t => new CompetitionProfileDto
                {
                    CompetitionId = t.CompetitionId,
                    CompetitionName = compNames.ContainsKey(t.CompetitionId) ? compNames[t.CompetitionId] : "Unknown Competition",
                    TeamName = t.Name,
                    Role = t.CaptainId == profile.Id ? "Captain" : t.Members.FirstOrDefault(m => m.StudentProfileId == profile.Id)?.Role ?? "Member",
                    Status = t.Status.ToString()
                }).ToList()
            };
        }
    }
}

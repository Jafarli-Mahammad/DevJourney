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
        private readonly IUniversityProfileRepository _universityProfileRepository;
        private readonly IProfessionRepository _professionRepository;
        private readonly IMainRoleRepository _mainRoleRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentProfileCommandHandler(
            IStudentProfileRepository studentProfileRepository,
            IUniversityProfileRepository universityProfileRepository,
            IProfessionRepository professionRepository,
            IMainRoleRepository mainRoleRepository,
            ISkillRepository skillRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _studentProfileRepository = studentProfileRepository;
            _universityProfileRepository = universityProfileRepository;
            _professionRepository = professionRepository;
            _mainRoleRepository = mainRoleRepository;
            _skillRepository = skillRepository;
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

            // 1. Resolve University (GUID or Slug/Name)
            Guid? resolvedUniversityId = null;
            if (!string.IsNullOrWhiteSpace(request.UniversityId))
            {
                if (Guid.TryParse(request.UniversityId, out var uniGuid))
                {
                    resolvedUniversityId = uniGuid;
                }
                else
                {
                    var rawUni = request.UniversityId.Trim().ToLowerInvariant();
                    var allUnis = await _universityProfileRepository.GetAllAsync(cancellationToken: cancellationToken);
                    var matchedUni = allUnis.FirstOrDefault(u =>
                        u.UniversityName.ToLowerInvariant().Contains(rawUni) ||
                        (rawUni == "bmu" && u.UniversityName.Contains("BMU")) ||
                        (rawUni == "aztu" && u.UniversityName.Contains("AzTU")) ||
                        (rawUni == "bdu" && u.UniversityName.Contains("BDU")) ||
                        (rawUni == "ada" && u.UniversityName.Contains("ADA")) ||
                        (rawUni == "bhos" && (u.UniversityName.Contains("BANM") || u.UniversityName.Contains("BHOS"))) ||
                        (rawUni == "banm" && u.UniversityName.Contains("BANM")) ||
                        (rawUni == "unec" && u.UniversityName.Contains("UNEC")) ||
                        (rawUni == "adnsu" && u.UniversityName.Contains("ADNSU")) ||
                        (rawUni == "khazar" && u.UniversityName.Contains("Khazar")) ||
                        (rawUni == "sdu" && u.UniversityName.Contains("SDU")) ||
                        (rawUni == "atu" && u.UniversityName.Contains("ATU")) ||
                        (rawUni == "adiu" && u.UniversityName.Contains("ADU")) ||
                        (rawUni == "adu" && u.UniversityName.Contains("ADU")) ||
                        (rawUni == "maa" && u.UniversityName.Contains("MAA")));
                    resolvedUniversityId = matchedUni?.Id;
                }
            }

            // 2. Resolve Profession (GUID or Name)
            Guid? resolvedProfessionId = null;
            if (!string.IsNullOrWhiteSpace(request.ProfessionId))
            {
                if (Guid.TryParse(request.ProfessionId, out var profGuid))
                {
                    resolvedProfessionId = profGuid;
                }
                else
                {
                    var rawProf = request.ProfessionId.Trim().ToLowerInvariant();
                    var allProfs = await _professionRepository.GetAllAsync(cancellationToken: cancellationToken);
                    var matchedProf = allProfs.FirstOrDefault(p =>
                        p.Name.ToLowerInvariant().Contains(rawProf) || rawProf.Contains(p.Name.ToLowerInvariant()));
                    resolvedProfessionId = matchedProf?.Id;
                }
            }

            // 3. Resolve MainRole (GUID or Name)
            Guid? resolvedMainRoleId = null;
            if (!string.IsNullOrWhiteSpace(request.MainRoleId))
            {
                if (Guid.TryParse(request.MainRoleId, out var roleGuid))
                {
                    resolvedMainRoleId = roleGuid;
                }
                else
                {
                    var rawRole = request.MainRoleId.Trim().ToLowerInvariant();
                    var allRoles = await _mainRoleRepository.GetAllAsync(cancellationToken: cancellationToken);
                    var matchedRole = allRoles.FirstOrDefault(r =>
                        r.Name.ToLowerInvariant().Contains(rawRole) || rawRole.Contains(r.Name.ToLowerInvariant()));
                    resolvedMainRoleId = matchedRole?.Id;
                }
            }

            // 4. Resolve Skills (GUIDs or Names)
            List<Guid>? resolvedSkillIds = null;
            if (request.SkillIds != null && request.SkillIds.Count > 0)
            {
                resolvedSkillIds = new List<Guid>();
                var allSkills = await _skillRepository.GetAllAsync(cancellationToken: cancellationToken);
                foreach (var rawSkill in request.SkillIds)
                {
                    if (string.IsNullOrWhiteSpace(rawSkill)) continue;
                    if (Guid.TryParse(rawSkill, out var sGuid))
                    {
                        resolvedSkillIds.Add(sGuid);
                    }
                    else
                    {
                        var rawSkillLower = rawSkill.Trim().ToLowerInvariant();
                        var matchedSkill = allSkills.FirstOrDefault(s =>
                            s.Name.ToLowerInvariant() == rawSkillLower || s.Name.ToLowerInvariant().Contains(rawSkillLower));
                        if (matchedSkill != null)
                        {
                            resolvedSkillIds.Add(matchedSkill.Id);
                        }
                    }
                }
            }

            profile.UpdateCabinetProfile(
                resolvedUniversityId,
                request.PhoneNumber,
                resolvedProfessionId,
                request.Course,
                request.GitHubUrl,
                request.LinkedinUrl,
                request.PortfolioUrl,
                request.CVUrl,
                resolvedMainRoleId,
                request.ExperienceLevel,
                request.Bio);

            profile.SetSkills(resolvedSkillIds);

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

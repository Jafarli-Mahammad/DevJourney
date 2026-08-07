using Domain.Models.Concrates;
using Domain.Models.Entities.University;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Student
{
    public class StudentProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; set; }

        // Basic info
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Guid? UniversityId { get; set; }
        public UniversityProfile? University { get; set; }

        // Education & Contact
        public string? PhoneNumber { get; set; }
        public Guid? ProfessionId { get; set; }
        public Profession? Profession { get; set; }
        public string? Course { get; set; }

        // Social & Portfolio links
        public string? GitHubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? CVUrl { get; set; }

        // Specialty & Skills
        public Guid? MainRoleId { get; set; }
        public MainRole? MainRole { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        
        // About / Bio
        public string? Bio { get; set; }

        private readonly List<StudentSkill> _studentSkills = new();
        private readonly List<StudentLanguage> _studentLanguages = new();

        public IReadOnlyCollection<StudentSkill> StudentSkills => _studentSkills.AsReadOnly();
        public IReadOnlyCollection<StudentLanguage> StudentLanguages => _studentLanguages.AsReadOnly();

        public StudentProfile() { } // EF Constructor

        public StudentProfile(
            Guid applicationUserId,
            string firstName,
            string lastName,
            Guid? universityId = null)
        {
            ApplicationUserId = applicationUserId;
            FirstName = firstName;
            LastName = lastName;
            UniversityId = universityId;
        }

        public void UpdateCabinetProfile(
            Guid? universityId,
            string? phoneNumber,
            Guid? professionId,
            string? course,
            string? githubUrl,
            string? linkedinUrl,
            string? portfolioUrl,
            string? cvUrl,
            Guid? mainRoleId,
            ExperienceLevel? experienceLevel,
            string? bio)
        {
            UniversityId = universityId;
            PhoneNumber = phoneNumber;
            ProfessionId = professionId;
            Course = course;
            GitHubUrl = githubUrl;
            LinkedinUrl = linkedinUrl;
            PortfolioUrl = portfolioUrl;
            CVUrl = cvUrl;
            MainRoleId = mainRoleId;
            ExperienceLevel = experienceLevel;
            Bio = bio;
        }

        public void SetSkills(IEnumerable<Guid>? skillIds)
        {
            _studentSkills.Clear();
            if (skillIds != null)
            {
                foreach (var skillId in skillIds.Distinct())
                {
                    _studentSkills.Add(new StudentSkill
                    {
                        StudentProfileId = Id,
                        SkillId = skillId
                    });
                }
            }
        }

        public void SetLanguages(IEnumerable<(Guid LanguageId, LanguageProficiencyLevel ProficiencyLevel)>? languages)
        {
            _studentLanguages.Clear();
            if (languages != null)
            {
                foreach (var (langId, level) in languages)
                {
                    _studentLanguages.Add(new StudentLanguage
                    {
                        StudentProfileId = Id,
                        LanguageId = langId,
                        ProficiencyLevel = level
                    });
                }
            }
        }

        public int CalculateProfileCompletionPercentage()
        {
            var checks = new bool[]
            {
                UniversityId.HasValue,
                !string.IsNullOrWhiteSpace(PhoneNumber),
                ProfessionId.HasValue,
                !string.IsNullOrWhiteSpace(Course),
                !string.IsNullOrWhiteSpace(GitHubUrl),
                !string.IsNullOrWhiteSpace(LinkedinUrl),
                !string.IsNullOrWhiteSpace(PortfolioUrl) || !string.IsNullOrWhiteSpace(CVUrl),
                MainRoleId.HasValue,
                ExperienceLevel.HasValue,
                !string.IsNullOrWhiteSpace(Bio),
                _studentSkills.Count > 0,
                _studentLanguages.Count > 0
            };

            int completedCount = checks.Count(c => c);
            return (int)Math.Round((double)completedCount / checks.Length * 100);
        }
    }
}
using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Student
{
    public class StudentProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; set; }
        public int Age { get; set; }
        public string? Location { get; set; }
        public PrimaryRole Role { get; set; }
        public string? CVUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public ExperienceLevel Experience { get; set; }
        public string Achievements { get; set; }
        public string? Bio { get; set; }
        public WorkFormat PreferredWorkFormat { get; set; }

        private readonly List<StudentSkill> _studentSkills = new();
        private readonly List<StudentLanguage> _studentLanguages = new();

        public IReadOnlyCollection<StudentSkill> StudentSkills => _studentSkills.AsReadOnly();
        public IReadOnlyCollection<StudentLanguage> StudentLanguages => _studentLanguages.AsReadOnly();

        public void AddSkill(Guid skillId)
        {
            if (_studentSkills.Any(s => s.SkillId == skillId)) return;
            _studentSkills.Add(new StudentSkill { SkillId = skillId, StudentProfileId = Id });
        }

        public void AddLanguage(Guid languageId, LanguageProficiencyLevel level)
        {
            if (_studentLanguages.Any(l => l.LanguageId == languageId)) return;
            _studentLanguages.Add(new StudentLanguage
            {
                LanguageId = languageId,
                StudentProfileId = Id,
                ProficiencyLevel = level
            });
        }
    }
}

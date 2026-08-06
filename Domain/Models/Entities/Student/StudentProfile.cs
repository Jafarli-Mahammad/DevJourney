using Domain.Models.Concrates;

public class StudentProfile : AuditableEntity
{
    public Guid ApplicationUserId { get; set; }
    
    // Registration-time fields (required)
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Guid? UniversityId { get; private set; } // Just the ID, nothing else
    
    // Profile completion fields (optional)
    public string? Bio { get; set; }
    public string? ProfessionalRole { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? CVUrl { get; set; }

    private StudentProfile() { } // EF
    
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
    
    public void CompleteProfile(
        string? bio,
        string? professionalRole,
        string? githubUrl,
        string? linkedinUrl,
        string? cvUrl)
    {
        Bio = bio;
        ProfessionalRole = professionalRole;
        GitHubUrl = githubUrl;
        LinkedinUrl = linkedinUrl;
        CVUrl = cvUrl;
    }
}



/*using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Student
{
    public class StudentProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string? Location { get; set; }
        //public PrimaryRole Role { get; set; }
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
}*/
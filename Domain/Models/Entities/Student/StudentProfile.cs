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
        public string? Bio {  get; set; }
        public WorkFormat PreferredWorkFormat { get; set; }
        public ICollection<StudentSkill> StudentSkills { get; set; }
        public ICollection<StudentLanguage> StudentLanguages { get; set; }
    }
}

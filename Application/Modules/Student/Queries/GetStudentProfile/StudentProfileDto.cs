using Application.Modules.Skills;
using Domain.Models.Enums;

namespace Application.Modules.Student.Queries.GetStudentProfile
{
    public class StudentProfileDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? Email { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public Guid? UniversityId { get; set; }
        public string? UniversityName { get; set; }

        public string? PhoneNumber { get; set; }
        public Guid? ProfessionId { get; set; }
        public string? ProfessionName { get; set; }
        public string? Course { get; set; }

        public string? GitHubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? CVUrl { get; set; }

        public Guid? MainRoleId { get; set; }
        public string? MainRoleName { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }

        public string? Bio { get; set; }
        public int CompletionPercentage { get; set; }

        public List<SkillDto> Skills { get; set; } = new();
        public List<StudentLanguageDto> Languages { get; set; } = new();
    }
}

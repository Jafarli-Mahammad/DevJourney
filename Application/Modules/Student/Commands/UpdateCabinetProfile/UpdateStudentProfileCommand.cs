using Application.Modules.Student.Queries.GetStudentProfile;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Student.Commands.UpdateCabinetProfile
{
    public class UpdateStudentProfileCommand : IRequest<StudentProfileDto>
    {
        public string? UniversityId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfessionId { get; set; }
        public string? Course { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? CVUrl { get; set; }
        public string? MainRoleId { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public string? Bio { get; set; }
        public List<string>? SkillIds { get; set; }
        public List<StudentLanguageDto>? Languages { get; set; }
    }
}

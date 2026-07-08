using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Student.Commands.Register
{
    public class RegisterStudentCommand : IRequest<Guid>
    {
        public int Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Location { get; set; }
        public PrimaryRole Role { get; set; }
        public string? CVUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public ExperienceLevel Experience { get; set; }
        public string Achievements { get; set; }
        public string? Bio { get; set; }
        public WorkFormat PreferredWorkFormat { get; set; }

        public List<Guid> SkillIds { get; set; }
        public List<StudentLanguageDto> Languages { get; set; }
    }
}

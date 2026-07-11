using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Student.Commands.Register
{
    public record StudentRegisterRequest(
        int Age,
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string Password,
        string? Location,
        string? CVUrl,
        string? LinkedinUrl,
        string? GitHubUrl,
        ExperienceLevel Experience,
        string Achievements,
        string? Bio,
        WorkFormat PreferredWorkFormat,
        List<Guid> SkillIds,
        List<StudentLanguageDto> Languages
    ) : IRequest<Guid>;
}

//using Domain.Models.Enums;
//using MediatR;

//namespace Application.Modules.Student.Commands.Register
//{
//    public class StudentRegisterRequest : IRequest<Guid>
//    {
//        public int Age { get; set; }
//        public string FirstName { get; set; } = null!;
//        public string LastName { get; set; } = null!;
//        public string UserName { get; set; } = null!;
//        public string Email { get; set; } = null!;
//        public string Password { get; set; } = null!;
//        public string? Location { get; set; }
//        //public PrimaryRole Role { get; set; }
//        public string? CVUrl { get; set; }
//        public string? LinkedinUrl { get; set; }
//        public string? GitHubUrl { get; set; }
//        public ExperienceLevel Experience { get; set; }
//        public string Achievements { get; set; } = null!;
//        public string? Bio { get; set; }
//        public WorkFormat PreferredWorkFormat { get; set; }

//        public List<Guid> SkillIds { get; set; } = [];
//        public List<StudentLanguageDto> Languages { get; set; } = [];
//    }
//}



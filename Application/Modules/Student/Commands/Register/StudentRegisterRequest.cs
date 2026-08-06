using MediatR;

public record StudentRegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    Guid? UniversityId = null
) : IRequest<Guid>;


/*using Domain.Models.Enums;
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
}*/
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Student.Commands.Register
{
    public record StudentRegisterRequest(
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string Password,
        string? CVUrl,
        string? LinkedinUrl,
        string? GitHubUrl,
        string? Bio
    ) : IRequest<Guid>;
}

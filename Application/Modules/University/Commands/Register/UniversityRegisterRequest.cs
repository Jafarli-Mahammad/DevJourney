using MediatR;

namespace Application.Modules.University.Commands.Register
{
    public record UniversityRegisterRequest (
    string UserName,
    string Email,
    string Password,
    string UniversityName,
    string? WebsiteUrl,
    string? Location,
    string? RepresentativeName,
    string? RepresentativeEmail
    ) : IRequest<Guid>;
}
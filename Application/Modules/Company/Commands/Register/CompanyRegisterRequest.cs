using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Company.Commands.Register
{
    public record CompanyRegisterRequest(
        string UserName,
        string Email,
        string Password,
        string CompanyName,
        CompanySize CompanySize,
        string? CompanySector,
        string? WebsiteUrl,
        string? LinkedInUrl,
        string? Location,
        string? RepresentativeName,
        string? RepresentativeEmail
    ) : IRequest<Guid>;
}
using Application.Modules.Auth.Models;
using MediatR;

using Application.Services;
using System.Security.Claims;
using System.Linq;

namespace Application.Modules.Auth.Commands.Login
{
    public class CompanyLoginRequest : IRequest<LoginResponseDto>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class CompanyLoginRequestHandler : IRequestHandler<CompanyLoginRequest, LoginResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly Application.Repositories.IPartnerProfileRepository _partnerProfileRepository;

        public CompanyLoginRequestHandler(IAuthService authService, IJwtService jwtService, Application.Repositories.IPartnerProfileRepository partnerProfileRepository)
        {
            _authService = authService;
            _jwtService = jwtService;
            _partnerProfileRepository = partnerProfileRepository;
        }

        public async Task<LoginResponseDto> Handle(CompanyLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _authService.CheckPasswordAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new Application.Exceptions.BadRequestException("Invalid email or password.");
            }

            var profiles = await _partnerProfileRepository.GetAllAsync(p => p.ApplicationUserId == user.Value.UserId, cancellationToken);
            var partnerProfile = profiles.FirstOrDefault();

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "COMPANY_ADMIN"),
                new Claim("companyId", partnerProfile?.Id.ToString() ?? "")
            };

            var token = _jwtService.GenerateAccessToken(user.Value.UserId, user.Value.UserName, user.Value.Email, claims);
            
            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserDto
                {
                    Id = user.Value.UserId,
                    Email = user.Value.Email,
                    FullName = partnerProfile?.PartnerName ?? user.Value.UserName,
                    Role = "Company",
                    CompanyId = partnerProfile?.Id,
                    RepresentativeName = partnerProfile?.RepresentativeName,
                    PartnerType = partnerProfile?.PartnerType.ToString(),
                    AvatarUrl = partnerProfile?.LogoUrl,
                    IsVerified = partnerProfile?.IsVerified ?? true
                }
            };
        }
    }
}

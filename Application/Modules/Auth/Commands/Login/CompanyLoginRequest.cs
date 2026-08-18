using Application.Modules.Auth.Models;
using MediatR;
using Application.Services;
using System.Security.Claims;

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

        public CompanyLoginRequestHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> Handle(CompanyLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _authService.CheckPasswordAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new Application.Exceptions.BadRequestException("Invalid email or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Company")
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
                    FullName = user.Value.UserName,
                    Role = "Company",
                    IsVerified = true
                }
            };
        }
    }
}

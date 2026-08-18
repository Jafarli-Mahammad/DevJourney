using Application.Modules.Auth.Models;
using Application.Services;
using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Commands.Login
{
    public class JuryLoginRequestHandler : IRequestHandler<JuryLoginRequest, LoginResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public JuryLoginRequestHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> Handle(JuryLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _authService.CheckPasswordByUserNameAsync(request.JuryCode, request.Password);
            if (user == null)
            {
                throw new Application.Exceptions.BadRequestException("Invalid jury code or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Jury")
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
                    Role = "Jury",
                    IsVerified = true
                }
            };
        }
    }
}

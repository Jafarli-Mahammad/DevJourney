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
        private readonly Application.Repositories.IJuryProfileRepository _juryProfileRepository;

        public JuryLoginRequestHandler(
            IAuthService authService,
            IJwtService jwtService,
            Application.Repositories.IJuryProfileRepository juryProfileRepository)
        {
            _authService = authService;
            _jwtService = jwtService;
            _juryProfileRepository = juryProfileRepository;
        }

        public async Task<LoginResponseDto> Handle(JuryLoginRequest request, CancellationToken cancellationToken)
        {
            var juries = await _juryProfileRepository.GetAllAsync(
                j => j.JuryCode == request.JuryCode || j.Email == request.JuryCode,
                cancellationToken);
            var jury = juries.FirstOrDefault();

            (Guid UserId, string UserName, string Email)? user = null;

            if (jury != null)
            {
                if (!string.IsNullOrEmpty(jury.Email))
                {
                    user = await _authService.CheckPasswordAsync(jury.Email, request.Password);
                }

                if (user == null && !string.IsNullOrEmpty(jury.JuryCode))
                {
                    user = await _authService.CheckPasswordByUserNameAsync(jury.JuryCode, request.Password);
                }
            }

            if (user == null)
            {
                user = await _authService.CheckPasswordByUserNameAsync(request.JuryCode, request.Password);
            }

            if (user == null)
            {
                user = await _authService.CheckPasswordAsync(request.JuryCode, request.Password);
            }

            if (user == null)
            {
                throw new Application.Exceptions.BadRequestException("Invalid jury code or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Jury"),
                new Claim(ClaimTypes.Role, "JURY")
            };

            if (jury?.CompetitionId != null)
            {
                claims.Add(new Claim("competitionId", jury.CompetitionId.Value.ToString()));
            }

            var token = _jwtService.GenerateAccessToken(user.Value.UserId, user.Value.UserName, user.Value.Email, claims);
            
            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserDto
                {
                    Id = user.Value.UserId,
                    Email = user.Value.Email,
                    FullName = jury?.FullName ?? user.Value.UserName,
                    Role = "Jury",
                    IsVerified = true
                }
            };
        }
    }
}

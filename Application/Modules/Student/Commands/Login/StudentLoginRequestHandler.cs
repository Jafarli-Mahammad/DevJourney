using Application.Modules.Auth.Models;
using Application.Services;
using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Student.Commands.Login
{
    public class StudentLoginRequestHandler : IRequestHandler<StudentLoginRequest, LoginResponseDto>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public StudentLoginRequestHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> Handle(StudentLoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _authService.CheckPasswordAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new Application.Exceptions.BadRequestException("Invalid email or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Student")
            };

            var token = _jwtService.GenerateAccessToken(user.Value.UserId, user.Value.UserName, user.Value.Email, claims);
            
            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // Assuming 24 hours expiry for now
                User = new UserDto
                {
                    Id = user.Value.UserId,
                    Email = user.Value.Email,
                    FullName = user.Value.UserName, // Or full name if available
                    Role = "Student",
                    IsVerified = true,
                    // AvatarUrl = ...,
                    // UniversityId = ...
                }
            };
        }
    }
}

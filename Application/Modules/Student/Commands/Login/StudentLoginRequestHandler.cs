using Application.Services;
using MediatR;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Student.Commands.Login
{
    public class StudentLoginRequestHandler : IRequestHandler<StudentLoginRequest, string>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public StudentLoginRequestHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        public async Task<string> Handle(StudentLoginRequest request, CancellationToken cancellationToken)
        {
            var isValid = await _authService.CheckPasswordAsync(request.Email, request.Password);
            if (!isValid)
            {
                throw new Application.Exceptions.BadRequestException("Invalid email or password.");
            }

            var user = await _authService.GetUserInfoByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Application.Exceptions.BadRequestException("User not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Student")
            };

            var token = _jwtService.GenerateAccessToken(user.Value.UserId, user.Value.UserName, user.Value.Email, claims);
            return token;
        }
    }
}

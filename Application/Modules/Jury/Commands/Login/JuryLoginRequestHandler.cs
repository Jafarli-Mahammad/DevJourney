using Application.Services;
using MediatR;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Jury.Commands.Login
{
    public class JuryLoginRequestHandler : IRequestHandler<JuryLoginRequest, string>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public JuryLoginRequestHandler(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        public async Task<string> Handle(JuryLoginRequest request, CancellationToken cancellationToken)
        {
            // Jury members login with their JuryCode (UserName) and password
            var isValid = await _authService.CheckPasswordByUserNameAsync(request.JuryCode, request.Password);
            if (!isValid)
            {
                throw new System.Exception("Invalid jury code or password.");
            }

            var user = await _authService.GetUserInfoByNameAsync(request.JuryCode);
            if (user == null)
            {
                throw new System.Exception("Jury user not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Jury")
            };

            var token = _jwtService.GenerateAccessToken(user.Value.UserId, user.Value.UserName, user.Value.Email, claims);
            return token;
        }
    }
}

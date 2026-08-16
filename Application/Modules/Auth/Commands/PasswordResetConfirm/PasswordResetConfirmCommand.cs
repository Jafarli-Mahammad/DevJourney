using Application.Exceptions;
using Application.Services;
using MediatR;

namespace Application.Modules.Auth.Commands.PasswordResetConfirm
{
    public class PasswordResetConfirmCommand : IRequest<bool>
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class PasswordResetConfirmCommandHandler : IRequestHandler<PasswordResetConfirmCommand, bool>
    {
        private readonly IAuthService _authService;

        public PasswordResetConfirmCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(PasswordResetConfirmCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.ToDictionary(x => "PasswordReset", x => (IEnumerable<string>)new[] { x });
                throw new BadRequestException("Password reset failed", errors);
            }

            return true;
        }
    }
}

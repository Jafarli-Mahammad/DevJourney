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
        public Task<bool> Handle(PasswordResetConfirmCommand request, CancellationToken cancellationToken)
        {
            // Just return true for MVP
            return Task.FromResult(true);
        }
    }
}

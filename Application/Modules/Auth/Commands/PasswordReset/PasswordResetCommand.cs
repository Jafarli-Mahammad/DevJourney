using MediatR;

namespace Application.Modules.Auth.Commands.PasswordReset
{
    public class PasswordResetCommand : IRequest<bool>
    {
        public string Email { get; set; } = null!;
    }

    public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, bool>
    {
        public Task<bool> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
        {
            // Just return true for MVP
            return Task.FromResult(true);
        }
    }
}

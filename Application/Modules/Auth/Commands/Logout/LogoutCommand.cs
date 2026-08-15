using MediatR;

namespace Application.Modules.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        public Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Just return true for MVP
            return Task.FromResult(true);
        }
    }
}

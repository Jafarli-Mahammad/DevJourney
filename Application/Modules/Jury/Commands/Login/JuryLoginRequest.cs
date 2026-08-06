using MediatR;

namespace Application.Modules.Jury.Commands.Login
{
    public class JuryLoginRequest : IRequest<string>
    {
        public string JuryCode { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

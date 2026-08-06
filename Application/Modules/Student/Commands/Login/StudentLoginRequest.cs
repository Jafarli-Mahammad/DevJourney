using MediatR;

namespace Application.Modules.Student.Commands.Login
{
    public class StudentLoginRequest : IRequest<string>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
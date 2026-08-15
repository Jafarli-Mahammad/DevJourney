using MediatR;

namespace Application.Modules.Student.Commands.Login
{
    public class StudentLoginRequest : IRequest<Application.Modules.Auth.Models.LoginResponseDto>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
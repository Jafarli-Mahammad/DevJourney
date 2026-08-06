using MediatR;
using System;

namespace Application.Modules.Jury.Commands.Register
{
    public class JuryRegisterRequest : IRequest<Guid>
    {
        public string JuryCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Specialization { get; set; }
        public Guid? CompetitionId { get; set; }
    }
}

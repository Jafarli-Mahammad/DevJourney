using MediatR;

namespace Application.Modules.Student.Commands.Verify;

public class VerifyStudentUniversityRequest : IRequest<bool>
{
    public Guid StudentProfileId { get; set; }
}
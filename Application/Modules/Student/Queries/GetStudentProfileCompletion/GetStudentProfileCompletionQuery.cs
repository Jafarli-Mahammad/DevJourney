using MediatR;

namespace Application.Modules.Student.Queries.GetStudentProfileCompletion
{
    public record GetStudentProfileCompletionQuery(Guid StudentId) : IRequest<ProfileCompletionDto>;
}

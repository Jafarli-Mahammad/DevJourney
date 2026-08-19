using Application.Modules.Student.Queries.GetStudentProfile;
using MediatR;

namespace Application.Modules.Student.Queries.GetMyStudentProfile
{
    public class GetMyStudentProfileQuery : IRequest<StudentProfileDto?>
    {
    }
}

using MediatR;
using System.Collections.Generic;

namespace Application.Modules.Student.Queries.GetAllStudentProfiles
{
    public class GetAllStudentProfilesQuery : IRequest<List<StudentProfileListDto>>
    {
    }
}

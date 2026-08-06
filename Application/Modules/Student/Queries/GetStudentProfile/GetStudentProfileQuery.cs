using MediatR;
using System;

namespace Application.Modules.Student.Queries.GetStudentProfile
{
    public class GetStudentProfileQuery : IRequest<StudentProfileDto>
    {
        public Guid Id { get; set; }

        public GetStudentProfileQuery(Guid id)
        {
            Id = id;
        }
    }
}

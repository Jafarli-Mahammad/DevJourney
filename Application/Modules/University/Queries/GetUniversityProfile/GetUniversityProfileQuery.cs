using MediatR;
using System;

namespace Application.Modules.University.Queries.GetUniversityProfile
{
    public class GetUniversityProfileQuery : IRequest<UniversityProfileDto>
    {
        public Guid Id { get; set; }

        public GetUniversityProfileQuery(Guid id)
        {
            Id = id;
        }
    }
}

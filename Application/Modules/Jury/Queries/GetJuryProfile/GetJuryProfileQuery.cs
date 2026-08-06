using MediatR;
using System;

namespace Application.Modules.Jury.Queries.GetJuryProfile
{
    public class GetJuryProfileQuery : IRequest<JuryProfileDto>
    {
        public Guid Id { get; set; }

        public GetJuryProfileQuery(Guid id)
        {
            Id = id;
        }
    }
}

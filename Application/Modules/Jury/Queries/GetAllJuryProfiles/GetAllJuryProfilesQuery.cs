using MediatR;
using System.Collections.Generic;

namespace Application.Modules.Jury.Queries.GetAllJuryProfiles
{
    public class GetAllJuryProfilesQuery : IRequest<List<GetJuryProfile.JuryProfileDto>>
    {
    }
}

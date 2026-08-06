using MediatR;
using System.Collections.Generic;

namespace Application.Modules.University.Queries.GetAllUniversityProfiles
{
    public class GetAllUniversityProfilesQuery : IRequest<List<GetUniversityProfile.UniversityProfileDto>>
    {
    }
}

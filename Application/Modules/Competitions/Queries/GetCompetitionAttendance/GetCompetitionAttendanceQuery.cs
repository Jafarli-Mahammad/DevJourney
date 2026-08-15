using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Competitions.Queries.GetCompetitionAttendance
{
    public class GetCompetitionAttendanceQuery : IRequest<object>
    {
        public Guid CompetitionId { get; set; }
    }

    public class GetCompetitionAttendanceQueryHandler : IRequestHandler<GetCompetitionAttendanceQuery, object>
    {
        public Task<object> Handle(GetCompetitionAttendanceQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(new { success = true, data = Array.Empty<object>() });
        }
    }
}

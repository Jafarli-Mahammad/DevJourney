using Application.Repositories;
using Application.Repositories.Competitions;
using MediatR;
using System;
using System.Linq;
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
        private readonly ICompetitionParticipantRepository _participantRepo;
        
        public GetCompetitionAttendanceQueryHandler(ICompetitionParticipantRepository participantRepo)
        {
            _participantRepo = participantRepo;
        }

        public async Task<object> Handle(GetCompetitionAttendanceQuery request, CancellationToken cancellationToken)
        {
            var participants = await _participantRepo.GetAllAsync(p => p.CompetitionId == request.CompetitionId, cancellationToken);
            
            var attendanceList = participants.Select(p => new
            {
                p.Id,
                p.Name,
                p.IsTeam,
                p.IsCheckedIn,
                p.CheckInTime
            }).ToList();

            return attendanceList;
        }
    }
}

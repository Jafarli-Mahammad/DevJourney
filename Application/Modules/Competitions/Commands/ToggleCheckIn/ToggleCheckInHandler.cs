using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Repositories.Competitions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Commands.ToggleCheckIn;

public class ToggleCheckInHandler : IRequestHandler<ToggleCheckInCommand, bool>
{
    private readonly ICompetitionParticipantRepository _participantRepository;
    private readonly ICompetitionTeamMemberRepository _teamMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleCheckInHandler(
        ICompetitionParticipantRepository participantRepository,
        ICompetitionTeamMemberRepository teamMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _participantRepository = participantRepository;
        _teamMemberRepository = teamMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ToggleCheckInCommand request, CancellationToken cancellationToken)
    {
        // Try individual
        var participant = await _participantRepository.GetAsync(
            p => p.CompetitionId == request.CompetitionId && p.IndividualStudentId == request.StudentId, 
            null,
            cancellationToken);

        if (participant != null && !participant.IsTeam)
        {
            participant.IsCheckedIn = !participant.IsCheckedIn;
            participant.CheckInTime = participant.IsCheckedIn ? DateTime.UtcNow : null;
            await _participantRepository.EditAsync(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Try team member
        var teamMember = await _teamMemberRepository.GetAsync(
            tm => tm.StudentProfileId == request.StudentId && tm.Participant.CompetitionId == request.CompetitionId,
            q => q.Include(t => t.Participant),
            cancellationToken);

        if (teamMember != null)
        {
            teamMember.IsCheckedIn = !teamMember.IsCheckedIn;
            teamMember.CheckInTime = teamMember.IsCheckedIn ? DateTime.UtcNow : null;
            
            await _teamMemberRepository.EditAsync(teamMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
}

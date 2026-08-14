using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
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
        var individualParticipants = await _participantRepository.GetAllAsync(
            p => p.CompetitionId == request.CompetitionId && p.IndividualStudentId == request.StudentId, 
            cancellationToken);

        var participant = individualParticipants.FirstOrDefault();
        if (participant != null && !participant.IsTeam)
        {
            participant.IsCheckedIn = !participant.IsCheckedIn;
            participant.CheckInTime = participant.IsCheckedIn ? DateTime.UtcNow : null;
            await _participantRepository.EditAsync(participant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Try team member
        var teamMembers = await _teamMemberRepository.GetAllAsync(
            tm => tm.StudentProfileId == request.StudentId && tm.Participant.CompetitionId == request.CompetitionId,
            cancellationToken);

        var teamMember = teamMembers.FirstOrDefault();
        if (teamMember != null)
        {
            teamMember.IsCheckedIn = !teamMember.IsCheckedIn;
            teamMember.CheckInTime = teamMember.IsCheckedIn ? DateTime.UtcNow : null;
            
            await _teamMemberRepository.EditAsync(teamMember);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        throw new NotFoundException("CompetitionParticipant", $"StudentId: {request.StudentId}, CompetitionId: {request.CompetitionId}");
    }
}


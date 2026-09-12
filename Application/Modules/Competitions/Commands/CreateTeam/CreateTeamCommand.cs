using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Repositories.Competitions;
using Application.Services;
using Domain.Models.Entities.Competition;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Competitions.Commands.CreateTeam;

public class CreateTeamCommand : IRequest<object>
{
    public Guid CompetitionId { get; set; }
    public string TeamName { get; set; } = string.Empty;
}

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, object>
{
    private readonly ICompetitionParticipantRepository _participantRepo;
    private readonly IStudentProfileRepository _studentProfileRepo;
    private readonly ICurrentUserService _currentUserService;

    public CreateTeamCommandHandler(
        ICompetitionParticipantRepository participantRepo,
        IStudentProfileRepository studentProfileRepo,
        ICurrentUserService currentUserService)
    {
        _participantRepo = participantRepo;
        _studentProfileRepo = studentProfileRepo;
        _currentUserService = currentUserService;
    }

    public async Task<object> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var profile = userId != Guid.Empty ? await _studentProfileRepo.GetByUserIdAsync(userId) : null;

        var participant = new CompetitionParticipant
        {
            Id = Guid.NewGuid(),
            CompetitionId = request.CompetitionId,
            Name = string.IsNullOrWhiteSpace(request.TeamName) ? "My Team" : request.TeamName,
            IsTeam = true,
            CaptainId = profile?.Id,
            AppliedAt = DateTime.UtcNow,
            Status = ApplicationStatus.Approved
        };

        if (profile != null)
        {
            participant.Members.Add(new CompetitionTeamMember
            {
                Id = Guid.NewGuid(),
                ParticipantId = participant.Id,
                StudentProfileId = profile.Id,
                Role = "Captain"
            });
        }

        await _participantRepo.AddAsync(participant, cancellationToken);

        return new { success = true, data = new { TeamId = participant.Id, TeamName = participant.Name } };
    }
}


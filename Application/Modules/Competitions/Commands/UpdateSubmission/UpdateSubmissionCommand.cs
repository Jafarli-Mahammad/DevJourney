using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Repositories.Competitions;
using Application.Services;
using MediatR;

namespace Application.Modules.Competitions.Commands.UpdateSubmission;

public class UpdateSubmissionCommand : IRequest<object>
{
    public Guid CompetitionId { get; set; }
    public string? GithubUrl { get; set; }
    public string? PitchDeckAssetId { get; set; }
}

public class UpdateSubmissionCommandHandler : IRequestHandler<UpdateSubmissionCommand, object>
{
    private readonly ICompetitionParticipantRepository _participantRepo;
    private readonly IStudentProfileRepository _studentProfileRepo;
    private readonly ICurrentUserService _currentUserService;

    public UpdateSubmissionCommandHandler(
        ICompetitionParticipantRepository participantRepo,
        IStudentProfileRepository studentProfileRepo,
        ICurrentUserService currentUserService)
    {
        _participantRepo = participantRepo;
        _studentProfileRepo = studentProfileRepo;
        _currentUserService = currentUserService;
    }

    public async Task<object> Handle(UpdateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var profile = userId != Guid.Empty ? await _studentProfileRepo.GetByUserIdAsync(userId) : null;

        if (profile != null)
        {
            var participants = await _participantRepo.GetAllAsync(
                cp => cp.CompetitionId == request.CompetitionId &&
                      (cp.CaptainId == profile.Id || cp.IndividualStudentId == profile.Id || cp.Members.Any(m => m.StudentProfileId == profile.Id)),
                cancellationToken);

            var participant = participants.FirstOrDefault();
            if (participant != null)
            {
                if (request.GithubUrl != null) participant.GithubUrl = request.GithubUrl;
                if (request.PitchDeckAssetId != null) participant.PitchDeckAssetId = request.PitchDeckAssetId;
                await _participantRepo.EditAsync(participant);
                return new { success = true, data = new { Message = "Submission updated successfully" } };
            }
        }

        return new { success = false, error = new { code = "NOT_FOUND", message = "Participant not found" } };
    }
}


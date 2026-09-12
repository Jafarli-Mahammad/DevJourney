using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Repositories.Competitions;
using Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Competitions.Queries.GetMyTeam;

public class GetMyTeamQuery : IRequest<object>
{
    public Guid CompetitionId { get; set; }
}

public class GetMyTeamQueryHandler : IRequestHandler<GetMyTeamQuery, object>
{
    private readonly ICompetitionParticipantRepository _participantRepo;
    private readonly IStudentProfileRepository _studentProfileRepo;
    private readonly ICurrentUserService _currentUserService;

    public GetMyTeamQueryHandler(
        ICompetitionParticipantRepository participantRepo,
        IStudentProfileRepository studentProfileRepo,
        ICurrentUserService currentUserService)
    {
        _participantRepo = participantRepo;
        _studentProfileRepo = studentProfileRepo;
        _currentUserService = currentUserService;
    }

    public async Task<object> Handle(GetMyTeamQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var profile = userId != Guid.Empty ? await _studentProfileRepo.GetByUserIdAsync(userId) : null;

        if (profile != null)
        {
            var participants = await _participantRepo.GetAllAsync(
                cp => cp.CompetitionId == request.CompetitionId &&
                      (cp.CaptainId == profile.Id || cp.IndividualStudentId == profile.Id || cp.Members.Any(m => m.StudentProfileId == profile.Id)),
                q => q.Include(cp => cp.Members),
                cancellationToken);

            var participant = participants.FirstOrDefault();
            if (participant != null)
            {
                return new
                {
                    success = true,
                    data = new
                    {
                        Id = participant.Id,
                        TeamName = participant.Name,
                        Status = participant.Status.ToString(),
                        IsCheckedIn = participant.IsCheckedIn,
                        ProjectName = participant.ProjectName,
                        GithubUrl = participant.GithubUrl,
                        PitchDeckAssetId = participant.PitchDeckAssetId,
                        Members = participant.Members.Select(m => new
                        {
                            m.Id,
                            m.StudentProfileId,
                            m.Role,
                            m.IsCheckedIn
                        }).ToList()
                    }
                };
            }
        }

        return new { success = true, data = (object?)null };
    }
}


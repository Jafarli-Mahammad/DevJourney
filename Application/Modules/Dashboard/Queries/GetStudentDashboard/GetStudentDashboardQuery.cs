using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Dashboard.Queries.GetStudentDashboard;

public class GetStudentDashboardQuery : IRequest<object>
{
}

public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, object>
{
    private readonly Application.Repositories.Core.ICertificateRepository _certificateRepository;
    private readonly Application.Repositories.Competitions.ICompetitionParticipantRepository _competitionParticipantRepository;
    private readonly Application.Services.ICurrentUserService _currentUserService;
    private readonly Application.Repositories.IStudentProfileRepository _studentProfileRepository;

    public GetStudentDashboardQueryHandler(
        Application.Repositories.Core.ICertificateRepository certificateRepository,
        Application.Repositories.Competitions.ICompetitionParticipantRepository competitionParticipantRepository,
        Application.Services.ICurrentUserService currentUserService,
        Application.Repositories.IStudentProfileRepository studentProfileRepository)
    {
        _certificateRepository = certificateRepository;
        _competitionParticipantRepository = competitionParticipantRepository;
        _currentUserService = currentUserService;
        _studentProfileRepository = studentProfileRepository;
    }

    public async Task<object> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return new { success = true, data = new { CertificatesCount = 0, ActiveCompetitions = 0, DeveloperXp = 0 } };
        }

        var certificates = await _certificateRepository.GetAllAsync(c => c.UserId == userId, cancellationToken);
        var certCount = certificates.Count();

        var profile = await _studentProfileRepository.GetByUserIdAsync(userId);
        int activeComps = 0;
        if (profile != null)
        {
            var comps = await _competitionParticipantRepository.GetAllAsync(cp => 
                cp.CaptainId == profile.Id || 
                cp.IndividualStudentId == profile.Id || 
                cp.Members.Any(m => m.StudentProfileId == profile.Id), cancellationToken);
            activeComps = comps.Count();
        }
        
        int xp = certCount * 50 + activeComps * 10;
        
        return new { success = true, data = new { CertificatesCount = certCount, ActiveCompetitions = activeComps, DeveloperXp = xp } };
    }
}

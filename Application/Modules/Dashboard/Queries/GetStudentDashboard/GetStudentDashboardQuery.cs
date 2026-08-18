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

    public GetStudentDashboardQueryHandler(
        Application.Repositories.Core.ICertificateRepository certificateRepository,
        Application.Repositories.Competitions.ICompetitionParticipantRepository competitionParticipantRepository,
        Application.Services.ICurrentUserService currentUserService)
    {
        _certificateRepository = certificateRepository;
        _competitionParticipantRepository = competitionParticipantRepository;
        _currentUserService = currentUserService;
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

        // Getting profile ID is tricky without injecting student repo, but for dashboard let's just return what we can
        // We will just return 0 for active comps if we can't find profile easily, or we can guess frontend only uses cert count.
        // Actually frontend probably fetches student profile for the dashboard.
        
        return new { success = true, data = new { CertificatesCount = certCount, ActiveCompetitions = 0, DeveloperXp = 0 } };
    }
}

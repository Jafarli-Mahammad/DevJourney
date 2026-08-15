using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Modules.Dashboard.Queries.GetStudentDashboard;

public class GetStudentDashboardQuery : IRequest<object>
{
}

public class GetStudentDashboardQueryHandler : IRequestHandler<GetStudentDashboardQuery, object>
{
    public Task<object> Handle(GetStudentDashboardQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<object>(new { success = true, data = new object[0] });
    }
}

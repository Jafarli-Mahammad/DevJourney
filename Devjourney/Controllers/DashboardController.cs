using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.Modules.Dashboard.Queries.GetStudentDashboard;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/student/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/student/dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            return Ok(await _mediator.Send(new GetStudentDashboardQuery()));
        }
    }
}

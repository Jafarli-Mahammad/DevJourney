using Application.Modules.Student.Queries.GetAllStudentProfiles;
using Application.Modules.Student.Queries.GetStudentProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetStudentProfile(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentProfileQuery(id), cancellationToken);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudentProfiles(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllStudentProfilesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

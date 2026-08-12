using Application.Modules.Jury.Queries.GetAllJuryProfiles;
using Application.Modules.Jury.Queries.GetJuryProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json", "application/problem+json")]
    public class JuryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JuryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetJuryProfile(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetJuryProfileQuery(id), cancellationToken);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJuryProfiles(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllJuryProfilesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

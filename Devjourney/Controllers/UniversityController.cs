using Application.Modules.University.Queries.GetAllUniversityProfiles;
using Application.Modules.University.Queries.GetUniversityProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UniversityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUniversityProfile(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUniversityProfileQuery(id), cancellationToken);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUniversityProfiles(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllUniversityProfilesQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

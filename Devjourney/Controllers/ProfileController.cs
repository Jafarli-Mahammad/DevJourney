using Application.Modules.Profile.Commands.UpdateProfile;
using Application.Modules.Profile.Commands.UploadCv;
using Application.Modules.Profile.Queries.GetMe;
using Application.Modules.Profile.Queries.GetMyProfile;
using Application.Modules.Profile.Queries.GetPublicProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces("application/json", "application/problem+json")]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        // [Authorize] // Commented for MVP/testing purposes unless strictly required
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMeQuery(), cancellationToken);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("me/profile")]
        // [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyProfileQuery(), cancellationToken);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("me/profile")]
        // [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateProfileCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("uploads/cv")]
        // [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadCv(
            [FromForm] UploadCvCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("public/profiles/{idOrSlug}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicProfile(
            [FromRoute] string idOrSlug,
            CancellationToken cancellationToken)
        {
            var query = new GetPublicProfileQuery { IdOrSlug = idOrSlug };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new { success = true, data = result });
        }
    }
}

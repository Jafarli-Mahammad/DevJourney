using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/partner/profile")]
    [Produces("application/json", "application/problem+json")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public class PartnerProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PartnerProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new Application.Modules.PartnerProfile.Queries.GetPartnerProfile.GetPartnerProfileQuery(), cancellationToken);
            return Ok(new { success = true, data = result });
        }

        [HttpPut]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile([FromBody] Application.Modules.PartnerProfile.Commands.UpdatePartnerProfile.UpdatePartnerProfileCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = true, data = result, message = "Partner profile updated successfully" });
        }
    }
}

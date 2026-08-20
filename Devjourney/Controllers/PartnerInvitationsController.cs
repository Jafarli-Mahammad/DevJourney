using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.PartnerAuth.Queries.VerifyInvitation;
using Application.Modules.PartnerAuth.Commands.RegisterPartner;
using Application.Modules.PartnerAuth.Commands.GenerateInvitation;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/partner-invitations")]
    [Produces("application/json", "application/problem+json")]
    public class PartnerInvitationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PartnerInvitationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{code}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        public async Task<IActionResult> VerifyInvitation(string code, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new VerifyInvitationQuery { Code = code }, cancellationToken);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("{code}/register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterPartner(string code, [FromBody] RegisterPartnerCommand request, CancellationToken cancellationToken)
        {
            request.ConfirmCode = code;
            var result = await _mediator.Send(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { success = true, data = result, message = "Partner account registered. Awaiting SuperAdmin verification." });
        }

        [HttpPost("generate-mock")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateMockInvitation([FromBody] GenerateMockInvitationCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = true, data = result });
        }
    }
}

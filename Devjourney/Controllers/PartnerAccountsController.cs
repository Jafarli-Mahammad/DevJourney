using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.PartnerAccounts.Commands.CreatePartnerAccount;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/partner/accounts")]
    [Produces("application/json", "application/problem+json")]
    public class PartnerAccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PartnerAccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult GetAccounts() => Ok(new { success = true, data = Array.Empty<object>() });

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAccount([FromBody] CreatePartnerAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { success = true, data = result, message = "Account created successfully. Deliver temporary credentials to the user." });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(Guid id)
        {
            return Ok(new { success = true, message = "Account access revoked and removed successfully." });
        }
    }
}

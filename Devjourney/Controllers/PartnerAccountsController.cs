using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Modules.PartnerAccounts.Commands.CreatePartnerAccount;
using Application.Modules.PartnerAccounts.Commands.DeletePartnerAccount;
using Application.Modules.PartnerAccounts.Queries.GetPartnerAccounts;

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
        public async Task<IActionResult> GetAccounts() 
        {
            var result = await _mediator.Send(new GetPartnerAccountsQuery());
            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAccount([FromBody] CreatePartnerAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, new { success = true, data = result, message = "Account created successfully. Deliver temporary credentials to the user." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            await _mediator.Send(new DeletePartnerAccountCommand { AccountId = id });
            return Ok(new { success = true, message = "Account access revoked and removed successfully." });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/support-tickets")]
    [ApiExplorerSettings(GroupName = "v1,partner,company")]
    [Produces("application/json", "application/problem+json")]
    public class SupportTicketsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSupportTickets() => Ok(new { data = Array.Empty<object>() });

        [HttpGet("/api/partner/Competitions/{competitionId}/support-tickets")]
        [ApiExplorerSettings(GroupName = "partner,company")]
        public IActionResult GetPartnerSupportTickets(Guid competitionId)
        {
            return Ok(new { success = true, data = Array.Empty<object>() });
        }

        [HttpPost("/api/partner/support-tickets/{ticketId}/messages")]
        [ApiExplorerSettings(GroupName = "partner,company")]
        public IActionResult ReplySupportTicket(Guid ticketId, [FromBody] object body)
        {
            return Ok(new { success = true, message = "Reply sent successfully." });
        }

        [HttpPatch("/api/partner/support-tickets/{ticketId}")]
        [ApiExplorerSettings(GroupName = "partner,company")]
        public IActionResult CloseSupportTicket(Guid ticketId, [FromBody] object body)
        {
            return Ok(new { success = true, message = "Ticket status updated." });
        }
    }
}

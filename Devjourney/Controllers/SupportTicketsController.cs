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
    }
}

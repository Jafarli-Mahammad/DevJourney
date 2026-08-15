using Microsoft.AspNetCore.Mvc;
using System;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/support-tickets")]
    [Produces("application/json", "application/problem+json")]
    public class SupportTicketsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSupportTickets() => Ok(new { data = Array.Empty<object>() });
    }
}

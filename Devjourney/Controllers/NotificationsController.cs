using Microsoft.AspNetCore.Mvc;
using System;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Produces("application/json", "application/problem+json")]
    public class NotificationsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetNotifications() => Ok(new { data = Array.Empty<object>() });
    }
}

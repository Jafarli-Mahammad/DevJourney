using Microsoft.AspNetCore.Mvc;
using System;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Produces("application/json", "application/problem+json")]
    public class AdminController : ControllerBase
    {
        [HttpGet("companies")]
        public IActionResult GetCompanies() => Ok(new { success = true, data = Array.Empty<object>() });

        [HttpGet("users")]
        public IActionResult GetUsers() => Ok(new { success = true, data = Array.Empty<object>() });

        [HttpGet("teams")]
        public IActionResult GetTeams() => Ok(new { success = true, data = Array.Empty<object>() });

        [HttpGet("supporters")]
        public IActionResult GetSupporters() => Ok(new { success = true, data = Array.Empty<object>() });

        [HttpGet("certificates")]
        public IActionResult GetCertificates() => Ok(new { success = true, data = Array.Empty<object>() });
    }
}

using Microsoft.AspNetCore.Mvc;
using System;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/partner/accounts")]
    [Produces("application/json", "application/problem+json")]
    public class PartnerAccountsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAccounts() => Ok(new { success = true, data = Array.Empty<object>() });

        [HttpPost]
        public IActionResult CreateAccount() => Ok(new { success = true, data = new object() });
    }
}

using Microsoft.AspNetCore.Mvc;
using System;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    [Produces("application/json", "application/problem+json")]
    public class CertificatesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCertificates() => Ok(new { data = Array.Empty<object>() });
    }
}

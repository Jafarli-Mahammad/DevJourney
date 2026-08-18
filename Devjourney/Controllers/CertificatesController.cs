using Microsoft.AspNetCore.Mvc;
using System;

using Application.Modules.Certificates.Commands.UploadCertificate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    [Produces("application/json", "application/problem+json")]
    public class CertificatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CertificatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult GetCertificates() => Ok(new { data = Array.Empty<object>() });

        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCertificate([FromForm] UploadCertificateCommand command)
        {
            var certificateId = await _mediator.Send(command);
            return Ok(new { success = true, certificateId });
        }
    }
}

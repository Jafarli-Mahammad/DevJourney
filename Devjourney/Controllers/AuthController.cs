using Application.Modules.Company.Commands.Register;
using Application.Modules.Student.Commands.Register;
using Application.Modules.University.Commands.Register;
using MediatR;
    using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent(
        [FromBody] StudentRegisterRequest request,
        CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, userId);
        }

        [HttpPost("register/company")]
        public async Task<IActionResult> RegisterCompany(
            [FromBody] CompanyRegisterRequest request,
            CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, userId);
        }

        [HttpPost("register/University")]
        public async Task<IActionResult> RegisterUniversity(
            [FromBody] UniversityRegisterRequest request,
            CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, userId);
        }
    }
}
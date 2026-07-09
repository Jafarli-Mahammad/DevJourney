using Application.Modules.Student.Commands.Register;
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
        [FromBody] RegisterStudentCommand command,
        CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(command, cancellationToken);
            return StatusCode(201, userId);
        }
    }
}

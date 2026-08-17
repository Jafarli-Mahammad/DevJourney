using Application.Modules.Company.Commands.Register;
using Application.Modules.Student.Commands.Register;
using Application.Modules.University.Commands.Register;
using Application.Modules.Student.Commands.Login;
using Application.Modules.Jury.Commands.Login;
using Application.Modules.Auth.Commands.Login;
using Application.Modules.Auth.Commands.Logout;
using Application.Modules.Auth.Commands.PasswordReset;
using Application.Modules.Auth.Commands.PasswordResetConfirm;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json", "application/problem+json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register/student")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterStudent(
        [FromBody] StudentRegisterRequest request,
        CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, userId);
        }

        [HttpPost("register/company")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterCompany(
            [FromBody] CompanyRegisterRequest request,
            CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, userId);
        }

        [HttpPost("register/University")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUniversity(
            [FromBody] UniversityRegisterRequest request,
            CancellationToken cancellationToken)
        {
            var userId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, userId);
        }

        [HttpPost("register/jury")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterJury(
            [FromBody] Application.Modules.Jury.Commands.Register.JuryRegisterRequest request,
            CancellationToken cancellationToken)
        {
            var profileId = await mediator.Send(request, cancellationToken);
            return StatusCode(201, profileId);
        }

        private void SetTokenCookie(string token, DateTime expiresAt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Must be true when using SameSiteMode.None
                SameSite = SameSiteMode.None, // Required for cross-domain requests
                Expires = expiresAt
            };
            Response.Cookies.Append("accessToken", token, cookieOptions);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(
            [FromBody] StudentLoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            SetTokenCookie(result.AccessToken, result.ExpiresAt);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("login/student")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginStudent(
            [FromBody] StudentLoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            SetTokenCookie(result.AccessToken, result.ExpiresAt);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("login/company")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginCompany(
            [FromBody] CompanyLoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            SetTokenCookie(result.AccessToken, result.ExpiresAt);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("login/jury")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginJury(
            [FromBody] JuryLoginRequest request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            SetTokenCookie(result.AccessToken, result.ExpiresAt);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new LogoutCommand(), cancellationToken);
            Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok(new { success = result });
        }

        [HttpPost("password-reset")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PasswordReset(
            [FromBody] PasswordResetCommand request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return Ok(new { success = result });
        }

        [HttpPost("password-reset/confirm")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PasswordResetConfirm(
            [FromBody] PasswordResetConfirmCommand request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);
            return Ok(new { success = result });
        }
    }
}
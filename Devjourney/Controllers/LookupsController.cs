using Application.Modules.IdeaFields.Queries.GetAll;
using Application.Modules.Languages.Queries.GetAll;
using Application.Modules.Roles.Queries.GetAll;
using Application.Modules.Skills.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly IMediator mediator;

        public LookupsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("skills")]
        public async Task<IActionResult> GetAllSkills(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllSkillsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetAllLanguages(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllLanguagesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllRolesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("IdeaFields")]
        public async Task<IActionResult> GetAllIdeaFields(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllIdeaFieldsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}

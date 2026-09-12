using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Application.Modules.Competitions.Queries.GetAvailableCompetitions;
using Application.Modules.Competitions.Queries.GetCompetitionDetails;
using Application.Modules.Competitions.Queries.GetMyTeam;
using Application.Modules.Competitions.Commands.CreateTeam;
using Application.Modules.Competitions.Commands.JoinTeam;
using Application.Modules.Competitions.Commands.UpdateSubmission;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json", "application/problem+json")]
    public class PublicCompetitionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicCompetitionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/competitions")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [Microsoft.AspNetCore.OutputCaching.OutputCache(PolicyName = "PublicListings")]
        public async Task<IActionResult> GetAvailableCompetitions()
        {
            var result = await _mediator.Send(new GetAvailableCompetitionsQuery());
            return Ok(result);
        }

        [HttpGet("/api/competitions/{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.OutputCaching.OutputCache(PolicyName = "PublicDetails")]
        public async Task<IActionResult> GetCompetitionDetails(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
            {
                return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Competition not found" } });
            }

            var result = await _mediator.Send(new GetCompetitionDetailsQuery { Id = guidId });
            if (result == null)
            {
                return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Competition not found" } });
            }

            return Ok(new { success = true, data = result });
        }

        [HttpGet("/api/competitions/{id:guid}/team")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyTeam(Guid id)
        {
            var result = await _mediator.Send(new GetMyTeamQuery { CompetitionId = id });
            return Ok(result);
        }

        [HttpPost("/api/competitions/{id:guid}/teams")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam(Guid id, [FromBody] CreateTeamCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("/api/competitions/{id:guid}/teams/join")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JoinTeam(Guid id, [FromBody] JoinTeamCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("/api/competitions/{id:guid}/submission")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}

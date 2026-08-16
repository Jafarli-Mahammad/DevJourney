using MediatR;
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
            return Ok(new { success = true, data = result });
        }

        [HttpGet("/api/competitions/{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [Microsoft.AspNetCore.OutputCaching.OutputCache(PolicyName = "PublicDetails")]
        public async Task<IActionResult> GetCompetitionDetails(Guid id)
        {
            var result = await _mediator.Send(new GetCompetitionDetailsQuery { Id = id });
            return Ok(new { success = true, data = result });
        }

        [HttpGet("/api/competitions/{id:guid}/team")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyTeam(Guid id)
        {
            var result = await _mediator.Send(new GetMyTeamQuery { CompetitionId = id });
            return Ok(new { success = true, data = result });
        }

        [HttpPost("/api/competitions/{id:guid}/teams")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam(Guid id, [FromBody] CreateTeamCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("/api/competitions/{id:guid}/teams/join")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JoinTeam(Guid id, [FromBody] JoinTeamCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("/api/competitions/{id:guid}/submission")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            return Ok(new { success = true, data = result });
        }
    }
}

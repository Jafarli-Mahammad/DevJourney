using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public class PublicCompetitionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicCompetitionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/competitions")]
        public async Task<IActionResult> GetAvailableCompetitions()
        {
            return Ok(await _mediator.Send(new GetAvailableCompetitionsQuery()));
        }

        [HttpGet("/api/competitions/{id}")]
        public async Task<IActionResult> GetCompetitionDetails(int id)
        {
            return Ok(await _mediator.Send(new GetCompetitionDetailsQuery { Id = id }));
        }

        [HttpGet("/api/competitions/{id}/team")]
        public async Task<IActionResult> GetMyTeam(int id)
        {
            return Ok(await _mediator.Send(new GetMyTeamQuery { CompetitionId = id }));
        }

        [HttpPost("/api/competitions/{id}/teams")]
        public async Task<IActionResult> CreateTeam(int id, [FromBody] CreateTeamCommand command)
        {
            command.CompetitionId = id;
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("/api/competitions/{id}/teams/join")]
        public async Task<IActionResult> JoinTeam(int id, [FromBody] JoinTeamCommand command)
        {
            command.CompetitionId = id;
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("/api/competitions/{id}/submission")]
        public async Task<IActionResult> UpdateSubmission(int id, [FromBody] UpdateSubmissionCommand command)
        {
            command.CompetitionId = id;
            return Ok(await _mediator.Send(command));
        }
    }
}

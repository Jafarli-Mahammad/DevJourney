using Application.Modules.Competitions.Commands.CreateCompetition;
using Application.Modules.Competitions.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/partner/[controller]")]
    [ApiExplorerSettings(GroupName = "partner")]
    // [Authorize(Roles = "Partner")] // Uncomment when roles are implemented
    public class CompetitionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompetitionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new competition
        /// </summary>
        [HttpPost("new")]
        public async Task<IActionResult> CreateCompetition([FromBody] CreateCompetitionDto dto)
        {
            // Usually we'd get the partner ID from the token like this:
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // var partnerId = Guid.Parse(userId); // Assuming PartnerId == UserId for now
            // But for testing purposes without auth we can just let it be empty or mock it.

            var command = new CreateCompetitionCommand
            {
                Dto = dto,
                PartnerId = Guid.NewGuid() // TODO: Replace with real partner ID
            };

            var result = await _mediator.Send(command);

            return Ok(new { Message = "Competition created successfully", CompetitionId = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerCompetitions()
        {
            // TODO: Replace with real partner ID from token
            var query = new Application.Modules.Competitions.Queries.GetPartnerCompetitions.GetPartnerCompetitionsQuery { PartnerId = Guid.NewGuid() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}/stages")]
        public async Task<IActionResult> GetCompetitionStages(Guid id)
        {
            var query = new Application.Modules.Competitions.Queries.GetCompetitionStages.GetCompetitionStagesQuery { CompetitionId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}/participants")]
        public async Task<IActionResult> GetCompetitionParticipants(Guid id, [FromQuery] Domain.Models.Enums.ApplicationStatus? status)
        {
            var query = new Application.Modules.Competitions.Queries.GetCompetitionParticipants.GetCompetitionParticipantsQuery { CompetitionId = id, Status = status };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("participants/{participantId}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(Guid participantId, [FromBody] Application.Modules.Competitions.Commands.UpdateApplicationStatus.UpdateApplicationStatusCommand command)
        {
            command.ParticipantId = participantId;
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpPost("{id}/check-in")]
        public async Task<IActionResult> ToggleCheckIn(Guid id, [FromBody] Application.Modules.Competitions.Commands.ToggleCheckIn.ToggleCheckInCommand command)
        {
            command.CompetitionId = id;
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return Ok(new { Message = "Check-in toggled successfully" });
        }

        [HttpGet("{id}/scoreboard")]
        public async Task<IActionResult> GetScoreboard(Guid id)
        {
            var query = new Application.Modules.Competitions.Queries.GetScoreboard.GetScoreboardQuery { CompetitionId = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

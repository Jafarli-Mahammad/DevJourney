using Application.Modules.Competitions.Commands.CreateCompetition;
using Application.Modules.Competitions.Commands.ToggleCheckIn;
using Microsoft.AspNetCore.Authorization;
using Application.Modules.Competitions.Commands.UpdateApplicationStatus;
using Application.Modules.Competitions.Dtos;
using Application.Modules.Competitions.Queries.GetCompetitionParticipants;
using Application.Modules.Competitions.Queries.GetCompetitionStages;
using Application.Modules.Competitions.Queries.GetPartnerCompetitions;
using Application.Modules.Competitions.Queries.GetScoreboard;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/partner/[controller]")]
    [ApiExplorerSettings(GroupName = "partner")]
    [Authorize]
    [Produces("application/json", "application/problem+json")]
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
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> CreateCompetition([FromBody] CreateCompetitionDto dto, CancellationToken cancellationToken)
        {
            var command = new CreateCompetitionCommand
            {
                Dto = dto,
                PartnerId = Guid.NewGuid() // Fallback to available partner if not found
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Competition created successfully", CompetitionId = result });
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PartnerCompetitionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerCompetitions(CancellationToken cancellationToken)
        {
            var query = new GetPartnerCompetitionsQuery { PartnerId = Guid.NewGuid() };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/stages")]
        [ProducesResponseType(typeof(List<Application.Modules.Competitions.Queries.GetCompetitionStages.CompetitionStageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompetitionStages(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCompetitionStagesQuery { CompetitionId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/participants")]
        [ProducesResponseType(typeof(List<CompetitionParticipantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompetitionParticipants(Guid id, [FromQuery] Domain.Models.Enums.ApplicationStatus? status, CancellationToken cancellationToken)
        {
            var query = new GetCompetitionParticipantsQuery { CompetitionId = id, Status = status };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("participants/{participantId}/status")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> UpdateApplicationStatus(Guid participantId, [FromBody] UpdateApplicationStatusRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateApplicationStatusCommand
            {
                ParticipantId = participantId,
                Status = request.Status
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result) return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Participant not found" } });
            return Ok(new { Message = "Status updated successfully" });
        }

        [HttpPost("{id}/check-in")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> ToggleCheckIn(Guid id, [FromBody] ToggleCheckInRequest request, CancellationToken cancellationToken)
        {
            var command = new ToggleCheckInCommand
            {
                CompetitionId = id,
                StudentId = request.StudentId
            };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result) return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Participant or team member not found" } });
            return Ok(new { Message = "Check-in toggled successfully" });
        }

        [HttpGet("{id}/scoreboard")]
        [ProducesResponseType(typeof(List<ScoreboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScoreboard(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetScoreboardQuery { CompetitionId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpGet("{id}/attendance")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompetitionAttendance(Guid id, CancellationToken cancellationToken)
        {
            var query = new Application.Modules.Competitions.Queries.GetCompetitionAttendance.GetCompetitionAttendanceQuery { CompetitionId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompetitionById(Guid id, CancellationToken cancellationToken)
        {
            var query = new Application.Modules.Competitions.Queries.GetCompetitionById.GetCompetitionByIdQuery { CompetitionId = id };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }

    public record UpdateApplicationStatusRequest(Domain.Models.Enums.ApplicationStatus Status);
    public record ToggleCheckInRequest(Guid StudentId);
}


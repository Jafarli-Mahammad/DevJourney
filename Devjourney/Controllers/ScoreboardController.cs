using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Application.Modules.Competitions.Queries.GetPublicScoreboard;
using Application.Modules.Competitions.Queries.GetMyResults;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json", "application/problem+json")]
    public class ScoreboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScoreboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/scoreboard")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [Microsoft.AspNetCore.OutputCaching.OutputCache(PolicyName = "PublicListings")]
        public async Task<IActionResult> GetScoreboard()
        {
            var result = await _mediator.Send(new GetPublicScoreboardQuery());
            return Ok(new { success = true, data = result });
        }

        [HttpGet("/api/competitions/{id:guid}/results/me")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyResults(Guid id)
        {
            var result = await _mediator.Send(new GetMyResultsQuery { CompetitionId = id });
            return Ok(new { success = true, data = result });
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.Modules.Competitions.Queries.GetPublicScoreboard;
using Application.Modules.Competitions.Queries.GetMyResults;

namespace Devjourney.Controllers
{
    [ApiController]
    public class ScoreboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScoreboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/api/scoreboard")]
        public async Task<IActionResult> GetScoreboard()
        {
            return Ok(await _mediator.Send(new GetPublicScoreboardQuery()));
        }

        [HttpGet("/api/competitions/{id}/results/me")]
        public async Task<IActionResult> GetMyResults(int id)
        {
            return Ok(await _mediator.Send(new GetMyResultsQuery { CompetitionId = id }));
        }
    }
}

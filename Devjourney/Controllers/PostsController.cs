using Application.Modules.Posts.Commands.CreateB2BCoursePromoPost;
using Application.Modules.Posts.Commands.CreateCorporateEventPost;
using Application.Modules.Posts.Commands.CreateNetworkingEventPost;
using Application.Modules.Posts.Commands.CreateTeamMemberSearchPost;
using Application.Modules.Posts.Commands.CreateTeamSearchPost;
using Application.Modules.Posts.Queries.GetB2BCoursePromoPost;
using Application.Modules.Posts.Queries.GetCorporateEventPost;
using Application.Modules.Posts.Queries.GetNetworkingEventPost;
using Application.Modules.Posts.Queries.GetTeamMemberSearchPost;
using Application.Modules.Posts.Queries.GetTeamSearchPost;
using Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Devjourney.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PostsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // ── Team Member Search ──────────────────────────────────────────────

        [HttpPost("team-member-search")]
        [Authorize]
        [ProducesResponseType(typeof(CreateTeamMemberSearchPostCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTeamMemberSearchPost(
            [FromBody] CreateTeamMemberSearchPostCommand command,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetTeamMemberSearchPost),
                new { id = response.Id },
                response);
        }

        [HttpGet("team-member-search")]
        [ProducesResponseType(typeof(PagedResult<TeamMemberSearchPostPagedItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeamMemberSearchPosts(
            [FromQuery] TeamMemberSearchPostPagedFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetTeamMemberSearchPostsQuery(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("team-member-search/{id:guid}")]
        [ProducesResponseType(typeof(TeamMemberSearchPostDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamMemberSearchPost(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetTeamMemberSearchPostDetailQuery(id), cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // ── Team Search ─────────────────────────────────────────────────────

        [HttpPost("team-search")]
        [Authorize]
        [ProducesResponseType(typeof(CreateTeamSearchPostCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTeamSearchPost(
            [FromBody] CreateTeamSearchPostCommand command,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetTeamSearchPost),
                new { id = response.Id },
                response);
        }

        [HttpGet("team-search")]
        [ProducesResponseType(typeof(PagedResult<TeamSearchPostPagedItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeamSearchPosts(
            [FromQuery] TeamSearchPostPagedFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetTeamSearchPostsQuery(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("team-search/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamSearchPost(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetTeamSearchPostDetailQuery(id), cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // ── Networking Events ───────────────────────────────────────────────

        [HttpPost("networking-events")]
        [Authorize]
        [ProducesResponseType(typeof(CreateNetworkingEventPostCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateNetworkingEventPost(
            [FromBody] CreateNetworkingEventPostCommand command,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetNetworkingEventPost),
                new { id = response.Id },
                response);
        }

        [HttpGet("networking-events")]
        [ProducesResponseType(typeof(PagedResult<NetworkingEventPostPagedItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNetworkingEventPosts(
            [FromQuery] NetworkingEventPostPagedFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetNetworkingEventPostsQuery(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("networking-events/{id:guid}")]
        [ProducesResponseType(typeof(NetworkingEventPostDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNetworkingEventPost(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetNetworkingEventPostDetailQuery(id), cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // ── Corporate Events ────────────────────────────────────────────────

        [HttpPost("corporate-events")]
        [Authorize]
        [ProducesResponseType(typeof(CreateCorporateEventPostCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCorporateEventPost(
            [FromBody] CreateCorporateEventPostCommand command,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetCorporateEventPost),
                new { id = response.Id },
                response);
        }

        [HttpGet("corporate-events")]
        [ProducesResponseType(typeof(PagedResult<CorporateEventPostPagedItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCorporateEventPosts(
            [FromQuery] CorporateEventPostPagedFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetCorporateEventPostsQuery(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("corporate-events/{id:guid}")]
        [ProducesResponseType(typeof(CorporateEventPostDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCorporateEventPost(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetCorporateEventPostDetailQuery(id), cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // ── B2B Course Promos ───────────────────────────────────────────────

        [HttpPost("b2b-course-promos")]
        [Authorize]
        [ProducesResponseType(typeof(CreateB2BCoursePromoPostCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateB2BCoursePromoPost(
            [FromBody] CreateB2BCoursePromoPostCommand command,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetB2BCoursePromoPost),
                new { id = response.Id },
                response);
        }

        [HttpGet("b2b-course-promos")]
        [ProducesResponseType(typeof(PagedResult<B2BCoursePromoPostPagedItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetB2BCoursePromoPosts(
            [FromQuery] B2BCoursePromoPostPagedFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetB2BCoursePromoPostsQuery(filter), cancellationToken);
            return Ok(result);
        }

        [HttpGet("b2b-course-promos/{id:guid}")]
        [ProducesResponseType(typeof(B2BCoursePromoPostDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetB2BCoursePromoPost(Guid id, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetB2BCoursePromoPostDetailQuery(id), cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }
    }
}

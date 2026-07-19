using Domain.Models.Entities.Post;
using MediatR;

namespace Application.Modules.Posts.Queries.GetTeamSearchPost
{
    public record GetTeamSearchPostDetailQuery(Guid Id) : IRequest<TeamSearchPost?>;
}

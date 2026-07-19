using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Notifications
{
    public record PostCreatedNotification(
        Guid PostId,
        Guid AuthorId,
        PostType Type
    ) : INotification;
}

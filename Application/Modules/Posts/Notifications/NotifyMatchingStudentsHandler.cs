using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Modules.Posts.Notifications
{
    public class NotifyMatchingStudentsHandler : INotificationHandler<PostCreatedNotification>
    {
        private readonly ILogger<NotifyMatchingStudentsHandler> _logger;

        public NotifyMatchingStudentsHandler(ILogger<NotifyMatchingStudentsHandler> logger)
            => _logger = logger;

        public Task Handle(PostCreatedNotification notification, CancellationToken ct)
        {
            // TODO: matching logic — deferred until Notifications feature exists
            _logger.LogInformation("Post {PostId} created by {AuthorId}",
                notification.PostId, notification.AuthorId);
            return Task.CompletedTask;
        }
    }
}

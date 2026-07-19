using Application.Modules.Posts.Notifications;
using Application.Repositories.Post;
using Application.Services;
using Domain.Models.Entities.Post;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateNetworkingEventPost
{
    public class CreateNetworkingEventPostCommandHandler : IRequestHandler<CreateNetworkingEventPostCommand, CreateNetworkingEventPostCommandResponse>
    {
        private readonly INetworkingEventPostRepository _networkingEventPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CreateNetworkingEventPostCommandHandler(
            INetworkingEventPostRepository networkingEventPostRepository,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _networkingEventPostRepository = networkingEventPostRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<CreateNetworkingEventPostCommandResponse> Handle(CreateNetworkingEventPostCommand request, CancellationToken cancellationToken)
        {
            var stops = new List<NetworkingEventStop>();
            if (request.Stops != null)
            {
                for (int i = 0; i < request.Stops.Count; i++)
                {
                    stops.Add(new NetworkingEventStop { Order = i, Description = request.Stops[i] });
                }
            }

            var post = new NetworkingEventPost
            {
                Id = Guid.NewGuid(),
                AuthorId = _currentUserService.UserId,
                Type = PostType.NetworkingEvent,
                OrganizationName = request.OrganizationName,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                MaxAttendees = request.MaxAttendees,
                TicketContact = request.TicketContact,
                IsPaid = request.IsPaid,
                Price = request.Price,
                Stops = stops
            };

            await _networkingEventPostRepository.AddAsync(post, cancellationToken);
            await _mediator.Publish(new PostCreatedNotification(post.Id, post.AuthorId, post.Type), cancellationToken);

            return new CreateNetworkingEventPostCommandResponse(post.Id);
        }
    }
}

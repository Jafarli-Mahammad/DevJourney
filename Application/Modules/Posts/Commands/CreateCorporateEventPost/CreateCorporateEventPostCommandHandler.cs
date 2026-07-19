using Application.Modules.Posts.Notifications;
using Application.Repositories.Post;
using Application.Services;
using Domain.Models.Entities.Post;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateCorporateEventPost
{
    public class CreateCorporateEventPostCommandHandler : IRequestHandler<CreateCorporateEventPostCommand, CreateCorporateEventPostCommandResponse>
    {
        private readonly ICorporateEventPostRepository _corporateEventPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CreateCorporateEventPostCommandHandler(
            ICorporateEventPostRepository corporateEventPostRepository,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _corporateEventPostRepository = corporateEventPostRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<CreateCorporateEventPostCommandResponse> Handle(CreateCorporateEventPostCommand request, CancellationToken cancellationToken)
        {
            var agenda = new List<CorporateEventAgendaItem>();
            if (request.Agenda != null)
            {
                for (int i = 0; i < request.Agenda.Count; i++)
                {
                    agenda.Add(new CorporateEventAgendaItem 
                    { 
                        Order = i, 
                        Time = request.Agenda[i].Time, 
                        Activity = request.Agenda[i].Activity 
                    });
                }
            }

            var post = new CorporateEventPost
            {
                Id = Guid.NewGuid(),
                AuthorId = _currentUserService.UserId,
                Type = PostType.CorporateEvent,
                Title = request.Title,
                EventTypeId = request.EventTypeId,
                SpecialOccasion = request.SpecialOccasion,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                TargetAudience = request.TargetAudience,
                MaxAttendees = request.MaxAttendees,
                ConfidentialityNote = request.ConfidentialityNote,
                ApplicationMethod = request.ApplicationMethod,
                ApplicationLink = request.ApplicationLink,
                EventLanguage = request.EventLanguage,
                IsPaid = request.IsPaid,
                Price = request.Price,
                Agenda = agenda
            };

            await _corporateEventPostRepository.AddAsync(post, cancellationToken);
            await _mediator.Publish(new PostCreatedNotification(post.Id, post.AuthorId, post.Type), cancellationToken);

            return new CreateCorporateEventPostCommandResponse(post.Id);
        }
    }
}

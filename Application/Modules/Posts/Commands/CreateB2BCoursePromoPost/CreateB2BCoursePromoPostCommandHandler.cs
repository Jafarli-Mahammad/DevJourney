using Application.Modules.Posts.Notifications;
using Application.Repositories.Post;
using Application.Services;
using Domain.Models.Entities.Post;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateB2BCoursePromoPost
{
    public class CreateB2BCoursePromoPostCommandHandler : IRequestHandler<CreateB2BCoursePromoPostCommand, CreateB2BCoursePromoPostCommandResponse>
    {
        private readonly IB2BCoursePromoPostRepository _b2bCoursePromoPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CreateB2BCoursePromoPostCommandHandler(
            IB2BCoursePromoPostRepository b2bCoursePromoPostRepository,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _b2bCoursePromoPostRepository = b2bCoursePromoPostRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<CreateB2BCoursePromoPostCommandResponse> Handle(CreateB2BCoursePromoPostCommand request, CancellationToken cancellationToken)
        {
            var post = new B2BCoursePromoPost
            {
                Id = Guid.NewGuid(),
                AuthorId = _currentUserService.UserId,
                Type = PostType.B2BCoursePromo,
                CourseName = request.CourseName,
                Title = request.Title,
                CourseTypeId = request.CourseTypeId,
                EventFormat = request.EventFormat,
                TargetMajor = request.TargetMajor,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                LocationOrLink = request.LocationOrLink,
                MaxAttendees = request.MaxAttendees,
                DurationInfo = request.DurationInfo,
                IsPaid = request.IsPaid,
                Price = request.Price,
                HasDiscount = request.HasDiscount,
                DiscountNote = request.DiscountNote,
                HasCertificate = request.HasCertificate,
                Content = request.Content,
                ApplicationMethod = request.ApplicationMethod,
                ApplicationLink = request.ApplicationLink,
                Instructor = new InstructorProfile
                {
                    Name = request.InstructorName,
                    LinkedIn = request.InstructorLinkedIn,
                    ReviewsLink = request.InstructorReviewsLink
                }
            };

            await _b2bCoursePromoPostRepository.AddAsync(post, cancellationToken);
            await _mediator.Publish(new PostCreatedNotification(post.Id, post.AuthorId, post.Type), cancellationToken);

            return new CreateB2BCoursePromoPostCommandResponse(post.Id);
        }
    }
}

using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Posts.Commands.CreateB2BCoursePromoPost
{
    public record CreateB2BCoursePromoPostCommand(
        string CourseName,
        string Title,
        Guid CourseTypeId,
        EventFormat EventFormat,
        string TargetMajor,
        DateTime StartAt,
        DateTime EndAt,
        string LocationOrLink,
        int MaxAttendees,
        string? DurationInfo,
        bool IsPaid,
        decimal? Price,
        bool HasDiscount,
        string? DiscountNote,
        bool? HasCertificate,
        string Content,
        ApplicationMethod ApplicationMethod,
        string? ApplicationLink,
        string? InstructorName,
        string? InstructorLinkedIn,
        string? InstructorReviewsLink
    ) : IRequest<CreateB2BCoursePromoPostCommandResponse>;

    public record CreateB2BCoursePromoPostCommandResponse(Guid Id);
}

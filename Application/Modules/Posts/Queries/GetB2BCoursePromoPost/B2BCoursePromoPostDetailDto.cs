using System;
using Domain.Models.Enums;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public record InstructorProfileDto(string? Name, string? LinkedIn, string? ReviewsLink);

    public record B2BCoursePromoPostDetailDto(
        Guid Id,
        Guid AuthorId,
        string AuthorName,
        string CourseName,
        string Title,
        Guid CourseTypeId,
        string CourseTypeName,
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
        InstructorProfileDto? Instructor,
        bool IsEdited,
        DateTime CreatedAt
    );
}

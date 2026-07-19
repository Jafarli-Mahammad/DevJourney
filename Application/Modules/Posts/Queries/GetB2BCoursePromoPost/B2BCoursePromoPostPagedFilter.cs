using System;
using Domain.Models.Enums;

namespace Application.Modules.Posts.Queries.GetB2BCoursePromoPost
{
    public record B2BCoursePromoPostPagedFilter(
        int Page,
        int PageSize,
        Guid? CourseTypeId,
        EventFormat? EventFormat,
        bool? IsPaid,
        string? TargetMajor,
        DateTime? StartAtMin,
        DateTime? StartAtMax
    );
}

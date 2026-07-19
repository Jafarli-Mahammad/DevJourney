using System;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public record CorporateEventPostPagedFilter(
        int Page,
        int PageSize,
        Guid? EventTypeId,
        string? CompanySector,
        DateTime? StartAtMin,
        DateTime? StartAtMax,
        bool? IsPaid,
        string? TargetMajor
    );
}

using System;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public record NetworkingEventPostPagedFilter(
        int Page,
        int PageSize,
        bool? IsPaid,
        string? Location,
        DateTime? StartAtMin,
        DateTime? StartAtMax
    );
}

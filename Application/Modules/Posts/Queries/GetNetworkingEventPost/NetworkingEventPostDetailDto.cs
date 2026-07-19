using System;
using System.Collections.Generic;

namespace Application.Modules.Posts.Queries.GetNetworkingEventPost
{
    public record NetworkingEventStopDto(int Order, string Description);

    public record NetworkingEventPostDetailDto(
        Guid Id,
        Guid AuthorId,
        string AuthorName,
        string OrganizationName,
        string Location,
        decimal? Latitude,
        decimal? Longitude,
        DateTime StartAt,
        DateTime EndAt,
        int MaxAttendees,
        string? TicketContact,
        bool IsPaid,
        decimal? Price,
        List<NetworkingEventStopDto> Stops,
        bool IsEdited,
        DateTime CreatedAt
    );
}

using MediatR;
using System.Collections.Generic;

namespace Application.Modules.Posts.Commands.CreateNetworkingEventPost
{
    public record CreateNetworkingEventPostCommand(
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
        List<string> Stops
    ) : IRequest<CreateNetworkingEventPostCommandResponse>;

    public record CreateNetworkingEventPostCommandResponse(Guid Id);
}

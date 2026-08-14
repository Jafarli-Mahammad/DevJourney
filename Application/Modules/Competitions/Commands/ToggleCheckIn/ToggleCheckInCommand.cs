using System;
using MediatR;

namespace Application.Modules.Competitions.Commands.ToggleCheckIn;

public class ToggleCheckInCommand : IRequest<bool>
{
    public Guid StudentId { get; set; }
    public Guid CompetitionId { get; set; }
}

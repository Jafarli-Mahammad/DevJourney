using System;
using Domain.Models.Enums;
using MediatR;

namespace Application.Modules.Competitions.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommand : IRequest<bool>
{
    public Guid ParticipantId { get; set; }
    public ApplicationStatus Status { get; set; }
}

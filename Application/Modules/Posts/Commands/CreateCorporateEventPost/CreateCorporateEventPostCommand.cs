using Application.Behaviors;
using Domain.Models.Enums;
using MediatR;
using System.Collections.Generic;

namespace Application.Modules.Posts.Commands.CreateCorporateEventPost
{
    public record CorporateEventAgendaItemCommandDto(string Time, string Activity);

    public record CreateCorporateEventPostCommand(
        string Title,
        Guid EventTypeId,
        string? SpecialOccasion,
        DateTime StartAt,
        DateTime EndAt,
        string Location,
        decimal? Latitude,
        decimal? Longitude,
        string TargetAudience,
        int MaxAttendees,
        string? ConfidentialityNote,
        ApplicationMethod ApplicationMethod,
        string? ApplicationLink,
        string? EventLanguage,
        bool IsPaid,
        decimal? Price,
        List<CorporateEventAgendaItemCommandDto>? Agenda
    ) : IRequest<CreateCorporateEventPostCommandResponse>, IRequireVerifiedAuthor;

    public record CreateCorporateEventPostCommandResponse(Guid Id);
}

using System;
using System.Collections.Generic;
using Domain.Models.Enums;

namespace Application.Modules.Posts.Queries.GetCorporateEventPost
{
    public record CorporateEventAgendaItemDto(int Order, string Time, string Activity);

    public record CorporateEventPostDetailDto(
        Guid Id,
        Guid AuthorId,
        string AuthorName,
        string Title,
        Guid EventTypeId,
        string EventTypeName,
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
        List<CorporateEventAgendaItemDto> Agenda,
        bool IsEdited,
        DateTime CreatedAt
    );
}

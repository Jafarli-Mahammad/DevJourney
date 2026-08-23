using System;
using System.Collections.Generic;
using MediatR;

namespace Application.Modules.Competitions.Queries.GetPartnerCompetitions;

public class GetPartnerCompetitionsQuery : IRequest<List<PartnerCompetitionDto>>
{
    public Guid PartnerId { get; set; }
}

public class PartnerCompetitionStageDto
{
    public Guid Id { get; set; }
    public int DayNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class PartnerCompetitionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? ShortSummary { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationDeadline { get; set; }
    public DateTime SubmissionDeadline { get; set; }
    public string? Location { get; set; }
    public string? LocationMapLink { get; set; }
    public string? Tags { get; set; }
    public string? EvaluationCriteria { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? BannerUrl => CoverImageUrl;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactSocialLink { get; set; }
    public int ParticipationFormat { get; set; }
    public int MaxTeamSize { get; set; }
    public bool IsPublished { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public bool IsJuryActive { get; set; }
    public bool IsScoreboardLive { get; set; }
    public bool IsCertificatesPublished { get; set; }
    public string AgendaMode { get; set; } = "MANUAL";
    public string? AgendaPdfUrl { get; set; }
    public List<PartnerCompetitionStageDto> Stages { get; set; } = new();

    public int ApplicantCount { get; set; }
    public int ApprovedCount { get; set; }
    public int CheckInCount { get; set; }
    public int TeamCount { get; set; }
}

using System;
using System.Collections.Generic;
using Domain.Models.Enums;

namespace Application.Modules.Competitions.Dtos
{
    public class CompetitionStageDetailsDto
    {
        public Guid Id { get; set; }
        public int DayNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class CompetitionDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortSummary { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime SubmissionDeadline { get; set; }
        public string? Location { get; set; }
        public string? LocationMapLink { get; set; }
        public string? CoverImageUrl { get; set; }
        public ParticipationFormat ParticipationFormat { get; set; }
        public int MaxTeamSize { get; set; }
        public string? Tags { get; set; }
        public string? EvaluationCriteria { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactSocialLink { get; set; }
        public RequirementLevel GitHubRepositoryRequirement { get; set; }
        public RequirementLevel LiveDeploymentRequirement { get; set; }
        public PitchDeckFormat PitchDeckFormat { get; set; }
        public string? AgendaMode { get; set; }
        public string? AgendaPdfUrl { get; set; }
        public bool IsPublished { get; set; }
        public bool IsRegistrationOpen { get; set; }
        public bool IsJuryActive { get; set; }
        public bool IsScoreboardLive { get; set; }
        public bool IsCertificatesPublished { get; set; }
        public string? PartnerName { get; set; }
        public List<CompetitionStageDetailsDto> Stages { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using Domain.Models.Abstracts;
using Domain.Models.Enums;
using Domain.Models.Entities.Partner;

namespace Domain.Models.Entities.Competition
{
    public class Competition : BaseEntity
    {
        public Guid PartnerId { get; set; }
        public PartnerProfile Partner { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string ShortSummary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ParticipationFormat ParticipationFormat { get; set; }
        public int MaxTeamSize { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }

        public string Location { get; set; } = string.Empty;
        public string LocationMapLink { get; set; } = string.Empty;
        
        public string Tags { get; set; } = string.Empty; // Can be serialized JSON or comma separated
        public string EvaluationCriteria { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactSocialLink { get; set; } = string.Empty;

        // Rules
        public DateTime SubmissionDeadline { get; set; }
        public RequirementLevel GitHubRepositoryRequirement { get; set; }
        public RequirementLevel LiveDeploymentRequirement { get; set; }
        public PitchDeckFormat PitchDeckFormat { get; set; }

        // Navigation
        public ICollection<CompetitionStage> Stages { get; set; } = new List<CompetitionStage>();
        
        // Status
        public bool IsPublished { get; set; }
    }
}

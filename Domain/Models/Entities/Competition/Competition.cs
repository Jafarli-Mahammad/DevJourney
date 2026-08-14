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
        public PartnerProfile Partner { get; set; }

        public string Title { get; set; }
        public string ShortSummary { get; set; }
        public string Description { get; set; }
        public ParticipationFormat ParticipationFormat { get; set; }
        public int MaxTeamSize { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RegistrationDeadline { get; set; }

        public string Location { get; set; }
        public string LocationMapLink { get; set; }
        
        public string Tags { get; set; } // Can be serialized JSON or comma separated
        public string EvaluationCriteria { get; set; }

        public string CoverImageUrl { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactSocialLink { get; set; }

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

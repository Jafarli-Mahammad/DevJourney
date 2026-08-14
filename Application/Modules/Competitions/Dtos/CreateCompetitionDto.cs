using System;
using System.Collections.Generic;
using Domain.Models.Enums;

namespace Application.Modules.Competitions.Dtos
{
    public class CompetitionStageDto
    {
        public int DayNumber { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class CreateCompetitionDto
    {
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
        public string Tags { get; set; }
        public string EvaluationCriteria { get; set; }
        public string CoverImageUrl { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactSocialLink { get; set; }
        public DateTime SubmissionDeadline { get; set; }
        public RequirementLevel GitHubRepositoryRequirement { get; set; }
        public RequirementLevel LiveDeploymentRequirement { get; set; }
        public PitchDeckFormat PitchDeckFormat { get; set; }
        public List<CompetitionStageDto> Stages { get; set; }
    }
}

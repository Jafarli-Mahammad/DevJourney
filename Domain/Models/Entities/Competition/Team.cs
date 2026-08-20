using System;
using System.Collections.Generic;
using Domain.Models.Concrates;

namespace Domain.Models.Entities.Competition
{
    public class Team : AuditableEntity
    {
        public Guid CompetitionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CaptainId { get; set; } // Reference to User/Student
        public bool IsFinalist { get; set; }
        public string? RepoUrl { get; set; }
        public string? PitchDeckUrl { get; set; }
        
        public virtual Competition Competition { get; set; } = null!;
        public virtual ICollection<CompetitionTeamMember> Members { get; set; } = new List<CompetitionTeamMember>();
    }
}

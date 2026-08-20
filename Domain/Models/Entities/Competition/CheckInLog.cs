using System;
using Domain.Models.Concrates;

namespace Domain.Models.Entities.Competition
{
    public class CheckInLog : AuditableEntity
    {
        public Guid CompetitionId { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid VerifiedBy { get; set; } // ApplicationUserId of the SUPPORTER
        public DateTime CheckedInAt { get; set; }
    }
}

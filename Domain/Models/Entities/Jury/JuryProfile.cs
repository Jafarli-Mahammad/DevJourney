using Domain.Models.Concrates;

namespace Domain.Models.Entities.Jury
{
    public class JuryProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; set; }
        
        public string JuryCode { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Specialization { get; set; }
        public Guid? CompetitionId { get; set; } // Can be linked to a specific competition later

        private JuryProfile() { } // EF Core

        public JuryProfile(Guid applicationUserId, string juryCode, string? fullName = null, string? email = null)
        {
            ApplicationUserId = applicationUserId;
            JuryCode = juryCode;
            FullName = fullName;
            Email = email;
        }

        public void UpdateProfile(string fullName, string specialization)
        {
            FullName = fullName;
            Specialization = specialization;
        }
    }
}

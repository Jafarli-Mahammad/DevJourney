using System;

namespace Application.Modules.Jury.Queries.GetJuryProfile
{
    public class JuryProfileDto
    {
        public Guid Id { get; set; }
        public string JuryCode { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Specialization { get; set; }
        public Guid? CompetitionId { get; set; }
    }
}

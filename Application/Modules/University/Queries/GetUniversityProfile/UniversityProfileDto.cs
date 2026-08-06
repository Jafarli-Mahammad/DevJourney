using System;

namespace Application.Modules.University.Queries.GetUniversityProfile
{
    public class UniversityProfileDto
    {
        public Guid Id { get; set; }
        public string UniversityName { get; set; } = null!;
        public string? WebsiteUrl { get; set; }
        public string? Location { get; set; }
        public string? RepresentativeName { get; set; }
        public string? RepresentativeEmail { get; set; }
        public bool IsVerified { get; set; }
    }
}

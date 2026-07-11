using Domain.Models.Concrates;

namespace Domain.Models.Entities.University
{
    public class UniversityProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; set; }
        public string UniversityName { get; set; } = null!;
        public string? WebsiteUrl { get; set; }
        public string? Location { get; set; }
        public string? RepresentativeName { get; set; }
        public string? RepresentativeEmail { get; set; }
        public bool IsVerified { get; set; }

        private UniversityProfile() { } // EF

        public UniversityProfile(Guid applicationUserId, string universityName)
        {
            ApplicationUserId = applicationUserId;
            UniversityName = universityName;
            IsVerified = false; // explicit: never trust-on-create
        }

        public void UpdateProfile(string? websiteUrl, string? location, string? representativeName, string? representativeEmail)
        {
            WebsiteUrl = websiteUrl;
            RepresentativeName = representativeName;
            RepresentativeEmail = representativeEmail;
            Location = location;
        }

        public void MarkVerified() => IsVerified = true; // named intent, callable only from a dedicated verification handler
    }
}
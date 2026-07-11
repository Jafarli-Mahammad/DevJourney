using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Company
{
    public class CompanyProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; private set; }
        public string CompanyName { get; private set; } = null!;
        public string? CompanySector { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public string? LinkedInUrl { get; private set; }
        public string? Location { get; private set; }
        public string? RepresentativeName { get; private set; }
        public string? RepresentativeEmail { get; private set; }
        public CompanySize CompanySize { get; private set; }
        public bool IsVerified { get; private set; }

        private CompanyProfile() { } // EF

        public CompanyProfile(Guid applicationUserId, string companyName, CompanySize companySize)
        {
            ApplicationUserId = applicationUserId;
            CompanyName = companyName;
            CompanySize = companySize;
            IsVerified = false; // explicit: never trust-on-create
        }

        public void UpdateProfile(string? sector, string? websiteUrl, string? linkedInUrl, string? location, string? representativeName, string? representativeEmail)
        {
            CompanySector = sector;
            WebsiteUrl = websiteUrl;
            LinkedInUrl = linkedInUrl;
            Location = location;
            RepresentativeName = representativeName;
            RepresentativeEmail = representativeEmail;
        }

        public void MarkVerified() => IsVerified = true; // named intent, callable only from a dedicated verification handler
    }
}
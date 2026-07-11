using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Partner
{
    public class PartnerProfile : AuditableEntity
    {
        public Guid ApplicationUserId { get; set; }
        public string PartnerName { get; set; } = null!;
        public PartnerType PartnerType { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public bool IsVerified { get; set; }
    }
}
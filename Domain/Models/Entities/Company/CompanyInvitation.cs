using System;
using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Company
{
    public class CompanyInvitation : AuditableEntity
    {
        public string Code { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public PartnerType PartnerType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}

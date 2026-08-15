using Domain.Models.Concrates;

namespace Domain.Models.Entities
{
    public class Role : AuditableEntity
    {
        public new Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

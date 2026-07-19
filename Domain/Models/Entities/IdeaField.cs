using Domain.Models.Concrates;

namespace Domain.Models.Entities
{
    public class IdeaField : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}

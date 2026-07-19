using Domain.Models.Concrates;

namespace Domain.Models.Entities.Post
{
    public class EventType : AuditableEntity
    {
        public string Name { get; set; } = null!;
    }
}

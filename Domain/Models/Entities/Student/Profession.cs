using Domain.Models.Concrates;

namespace Domain.Models.Entities.Student
{
    public class Profession : AuditableEntity
    {
        public string Name { get; set; } = null!;
    }
}

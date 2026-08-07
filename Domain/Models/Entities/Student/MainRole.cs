using Domain.Models.Concrates;

namespace Domain.Models.Entities.Student
{
    public class MainRole : AuditableEntity
    {
        public string Name { get; set; } = null!;
    }
}

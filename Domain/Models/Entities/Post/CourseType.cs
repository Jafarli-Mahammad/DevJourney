using Domain.Models.Concrates;

namespace Domain.Models.Entities.Post
{
    public class CourseType : AuditableEntity
    {
        public string Name { get; set; } = null!;
    }
}

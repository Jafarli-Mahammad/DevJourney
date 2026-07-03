using Domain.Models.Concrates;

namespace Domain.Models.Entities.Student
{
    public class Langauge : AuditableEntity
    {
        public string Name { get; set; }

        public ICollection<StudentSkill> StudentSkills { get; set; }
    }
}

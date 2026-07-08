using Domain.Models.Concrates;

namespace Domain.Models.Entities.Student
{
    public class Skill : AuditableEntity
    {
        public string Name { get; set; }

        public ICollection<StudentSkill> StudentSkills { get; set; }
    }
}

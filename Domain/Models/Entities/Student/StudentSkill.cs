namespace Domain.Models.Entities.Student
{
    public class StudentSkill
    {
        public Guid StudentProfileId { get; set; }

        public StudentProfile StudentProfile { get; set; }
        public Guid SkillId { get; set; }
        public Langauge Skill { get; set; }
    }
}

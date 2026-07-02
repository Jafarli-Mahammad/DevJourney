using Domain.Models.Concrates;

namespace Domain.Models.Entities.Student
{
    public class StudentLanguage
    {
        public Guid StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public Guid LanguageId { get; set; }
        public Language Language { get; set; }

        public string ProficiencyLevel { get; set; }

    }
}

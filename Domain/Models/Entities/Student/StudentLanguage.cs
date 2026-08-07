using Domain.Models.Enums;

namespace Domain.Models.Entities.Student
{
    public class StudentLanguage
    {
        public Guid StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; } = null!;

        public Guid LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public LanguageProficiencyLevel ProficiencyLevel { get; set; }
    }
}

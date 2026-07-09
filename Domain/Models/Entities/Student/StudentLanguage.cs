using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Student
{
    public class StudentLanguage
    {
        public Guid StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public Guid LanguageId { get; set; }
        public Language Language { get; set; }

        public LanguageProficiencyLevel ProficiencyLevel { get; set; }

    }
}

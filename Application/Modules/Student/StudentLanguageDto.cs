using Domain.Models.Enums;

namespace Application.Modules.Student
{
    public class StudentLanguageDto
    {
        public Guid LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public LanguageProficiencyLevel ProficiencyLevel { get; set; }
    }
}

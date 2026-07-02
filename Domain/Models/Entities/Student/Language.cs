using Domain.Models.Concrates;

namespace Domain.Models.Entities.Student
{
    public class Language : AuditableEntity
    {
        public string Name { get; set; }

        public ICollection<StudentLanguage> StudentLanguages { get; set; }
    }
}
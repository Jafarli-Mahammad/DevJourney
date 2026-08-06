namespace Domain.Models.Entities.Student
{
    public class StudentProfile : AuditableEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string University { get; set; }
        public string Password { get; set; }
    }
}

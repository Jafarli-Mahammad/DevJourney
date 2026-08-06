using System;

namespace Application.Modules.Student.Queries.GetStudentProfile
{
    public class StudentProfileDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? Email { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Guid? UniversityId { get; set; }
        public string? Bio { get; set; }
        public string? ProfessionalRole { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? CVUrl { get; set; }
    }
}

using System;

namespace Application.Modules.Student.Queries.GetAllStudentProfiles
{
    public class StudentProfileListDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string? Email { get; set; }
        public Guid? UniversityId { get; set; }
        public string? Bio { get; set; }
        public string? ProfessionalRole { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
    }
}

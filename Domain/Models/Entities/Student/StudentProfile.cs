namespace Domain.Models.Entities.Student
{
    public class StudentProfile : AuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Role { get; set; }
        public string CVUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GitHubUrl { get; set; }
        public string Experience { get; set; }
        public string Achievements { get; set; }
        public string Bio { get; set; }
        public string PreferredWorkFormat { get; set; }
        public string ApplicationUserId { get; set; }

        // Reverted the commented properties
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string University { get; set; }
        public string Password { get; set; }
    }
}

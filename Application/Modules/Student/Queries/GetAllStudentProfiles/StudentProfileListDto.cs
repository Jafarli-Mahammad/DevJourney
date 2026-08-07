namespace Application.Modules.Student.Queries.GetAllStudentProfiles
{
    public class StudentProfileListDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public Guid? UniversityId { get; set; }
        public string? UniversityName { get; set; }
        public Guid? ProfessionId { get; set; }
        public string? ProfessionName { get; set; }
        public Guid? MainRoleId { get; set; }
        public string? MainRoleName { get; set; }
        public string? Bio { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public int CompletionPercentage { get; set; }
    }
}

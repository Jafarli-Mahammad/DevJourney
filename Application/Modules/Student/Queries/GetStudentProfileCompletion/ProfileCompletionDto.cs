namespace Application.Modules.Student.Queries.GetStudentProfileCompletion
{
    public class ProfileCompletionDto
    {
        public Guid StudentProfileId { get; set; }
        public int CompletionPercentage { get; set; }
    }
}

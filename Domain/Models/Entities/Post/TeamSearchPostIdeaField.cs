namespace Domain.Models.Entities.Post
{
    public class TeamSearchPostIdeaField
    {
        public Guid PostId { get; set; }
        public TeamSearchPost Post { get; set; } = null!;

        public Guid IdeaFieldId { get; set; }
        public IdeaField IdeaField { get; set; } = null!;
    }
}

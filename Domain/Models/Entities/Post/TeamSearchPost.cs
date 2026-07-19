namespace Domain.Models.Entities.Post
{
    public class TeamSearchPost : Post
    {
        public string? Note { get; set; }

        public ICollection<TeamSearchPostIdeaField> IdeaFields { get; set; } = new List<TeamSearchPostIdeaField>();
    }
}

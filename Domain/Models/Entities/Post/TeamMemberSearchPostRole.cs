namespace Domain.Models.Entities.Post
{
    public class TeamMemberSearchPostRole
    {
        public Guid PostId { get; set; }
        public TeamMemberSearchPost Post { get; set; } = null!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}

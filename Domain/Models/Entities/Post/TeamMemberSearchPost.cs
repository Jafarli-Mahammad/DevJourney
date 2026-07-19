using Domain.Models.Entities.Student;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Post
{
    public class TeamMemberSearchPost : Post
    {
        public Guid IdeaFieldId { get; set; }
        public IdeaField IdeaField { get; set; } = null!;

        public string TeamName { get; set; } = null!;
        public int MembersNeeded { get; set; }
        public WorkFormat WorkMode { get; set; }
        public string SocialLink { get; set; } = null!;

        public int? LookingForAge { get; set; }
        public string? LookingForLocation { get; set; }
        public Guid? LookingForLanguageId { get; set; }
        public Language? LookingForLanguage { get; set; }

        public string? AdditionalNote { get; set; }

        public ICollection<TeamMemberSearchPostRole> TargetRoles { get; set; } = new List<TeamMemberSearchPostRole>();
        public ICollection<TeamMemberSearchPostSkill> TargetSkills { get; set; } = new List<TeamMemberSearchPostSkill>();
    }
}

using Domain.Models.Concrates;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Post
{
    public class Post : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; } // Fk => AppicationUser
        public PostType Type { get; set; }
        public bool IsEdited { get; set; } = false;
    }
}

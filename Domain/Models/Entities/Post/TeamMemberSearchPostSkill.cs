using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Entities.Post
{
    public class TeamMemberSearchPostSkill
    {
        public Guid PostId { get; set; }
        public Guid SkillId { get; set; }
    }
}

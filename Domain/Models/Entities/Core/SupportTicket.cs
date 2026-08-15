using System;
using System.Collections.Generic;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Core;

public class SupportTicket : BaseEntity
{
    public Guid UserId { get; set; }
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    
    public virtual ICollection<SupportMessage> Messages { get; set; } = new List<SupportMessage>();
}

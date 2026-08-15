using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Core;

public class SupportMessage : BaseEntity
{
    public Guid SupportTicketId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    
    public virtual SupportTicket SupportTicket { get; set; } = null!;
}

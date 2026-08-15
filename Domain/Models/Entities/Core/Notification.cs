using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Core;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
}

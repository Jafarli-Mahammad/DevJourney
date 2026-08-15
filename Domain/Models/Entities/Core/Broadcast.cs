using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Core;

public class Broadcast : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? TargetAudience { get; set; }
}

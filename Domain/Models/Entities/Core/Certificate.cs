using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Core;

public class Certificate : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? AssetId { get; set; }
}

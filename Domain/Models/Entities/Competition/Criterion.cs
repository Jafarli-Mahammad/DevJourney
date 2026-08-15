using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Competition;

public class Criterion : BaseEntity
{
    public Guid CompetitionId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int MaxScore { get; set; }
    public int Weight { get; set; }

    public virtual Competition Competition { get; set; } = null!;
}

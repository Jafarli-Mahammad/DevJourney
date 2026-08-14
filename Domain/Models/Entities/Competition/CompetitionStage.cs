using System;
using Domain.Models.Abstracts;

namespace Domain.Models.Entities.Competition
{
    public class CompetitionStage : BaseEntity
    {
        public Guid CompetitionId { get; set; }
        public Competition Competition { get; set; }

        public int DayNumber { get; set; } // 1, 2, 3
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}

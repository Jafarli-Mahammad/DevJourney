using System;

namespace Application.Common.Models.RealTime
{
    public sealed class ScoreboardDeltaDto
    {
        public Guid CompetitionId { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public decimal NewScore { get; set; }
        public decimal ScoreDelta { get; set; }
        public int NewRank { get; set; }
        public int PreviousRank { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }

    public sealed class CompetitionPhaseNotificationDto
    {
        public Guid CompetitionId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public sealed class CompetitionAnnouncementDto
    {
        public Guid CompetitionId { get; set; }
        public Guid AnnouncementId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Severity { get; set; } = "info"; // "info", "warning", "critical"
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }

    public sealed class TimerSyncDto
    {
        public Guid CompetitionId { get; set; }
        public long RemainingSeconds { get; set; }
        public bool IsFrozen { get; set; }
        public DateTimeOffset ServerTimestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}

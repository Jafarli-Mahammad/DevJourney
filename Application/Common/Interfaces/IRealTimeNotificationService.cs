using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models.RealTime;

namespace Application.Common.Interfaces
{
    public interface IRealTimeNotificationService
    {
        Task BroadcastScoreboardDeltaAsync(Guid competitionId, ScoreboardDeltaDto delta, CancellationToken cancellationToken = default);
        Task BroadcastPhaseChangeAsync(Guid competitionId, CompetitionPhaseNotificationDto phase, CancellationToken cancellationToken = default);
        Task BroadcastAnnouncementAsync(Guid competitionId, CompetitionAnnouncementDto announcement, CancellationToken cancellationToken = default);
        Task BroadcastTimerSyncAsync(Guid competitionId, TimerSyncDto timer, CancellationToken cancellationToken = default);
    }
}

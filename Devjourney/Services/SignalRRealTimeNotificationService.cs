using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models.RealTime;
using Devjourney.Hubs;
using Devjourney.Hubs.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Devjourney.Services
{
    public class SignalRRealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<ScoreboardHub, IScoreboardClient> _scoreboardHub;
        private readonly IHubContext<CompetitionHub, ICompetitionClient> _competitionHub;

        public SignalRRealTimeNotificationService(
            IHubContext<ScoreboardHub, IScoreboardClient> scoreboardHub,
            IHubContext<CompetitionHub, ICompetitionClient> competitionHub)
        {
            _scoreboardHub = scoreboardHub;
            _competitionHub = competitionHub;
        }

        public async Task BroadcastScoreboardDeltaAsync(Guid competitionId, ScoreboardDeltaDto delta, CancellationToken cancellationToken = default)
        {
            var groupName = ScoreboardHub.GetGroupName(competitionId);
            await _scoreboardHub.Clients.Group(groupName).ReceiveScoreboardDelta(delta);
        }

        public async Task BroadcastPhaseChangeAsync(Guid competitionId, CompetitionPhaseNotificationDto phase, CancellationToken cancellationToken = default)
        {
            var groupName = CompetitionHub.GetGroupName(competitionId);
            await _competitionHub.Clients.Group(groupName).ReceivePhaseChange(phase);
        }

        public async Task BroadcastAnnouncementAsync(Guid competitionId, CompetitionAnnouncementDto announcement, CancellationToken cancellationToken = default)
        {
            var groupName = CompetitionHub.GetGroupName(competitionId);
            await _competitionHub.Clients.Group(groupName).ReceiveAnnouncement(announcement);
        }

        public async Task BroadcastTimerSyncAsync(Guid competitionId, TimerSyncDto timer, CancellationToken cancellationToken = default)
        {
            var groupName = CompetitionHub.GetGroupName(competitionId);
            await _competitionHub.Clients.Group(groupName).ReceiveTimerSync(timer);
        }
    }
}

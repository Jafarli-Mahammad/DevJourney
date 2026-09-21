using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models.RealTime;
using Devjourney.Hubs;
using Devjourney.Hubs.Contracts;
using Devjourney.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace DevJourney.Tests.Services
{
    public class SignalRRealTimeNotificationServiceTests
    {
        [Fact]
        public async Task BroadcastScoreboardDeltaAsync_DispatchesDeltaToCompetitionGroup()
        {
            // Arrange
            var scoreboardHubMock = new Mock<IHubContext<ScoreboardHub, IScoreboardClient>>();
            var competitionHubMock = new Mock<IHubContext<CompetitionHub, ICompetitionClient>>();
            var clientsMock = new Mock<IHubClients<IScoreboardClient>>();
            var clientMock = new Mock<IScoreboardClient>();

            var competitionId = Guid.NewGuid();
            var groupName = ScoreboardHub.GetGroupName(competitionId);

            scoreboardHubMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(groupName)).Returns(clientMock.Object);

            var service = new SignalRRealTimeNotificationService(scoreboardHubMock.Object, competitionHubMock.Object);
            var delta = new ScoreboardDeltaDto
            {
                CompetitionId = competitionId,
                TeamId = Guid.NewGuid(),
                TeamName = "Alpha Team",
                NewScore = 95.5m,
                ScoreDelta = 10m,
                NewRank = 1,
                PreviousRank = 2
            };

            // Act
            await service.BroadcastScoreboardDeltaAsync(competitionId, delta);

            // Assert
            clientMock.Verify(c => c.ReceiveScoreboardDelta(delta), Times.Once);
        }

        [Fact]
        public async Task BroadcastPhaseChangeAsync_DispatchesPhaseToCompetitionGroup()
        {
            // Arrange
            var scoreboardHubMock = new Mock<IHubContext<ScoreboardHub, IScoreboardClient>>();
            var competitionHubMock = new Mock<IHubContext<CompetitionHub, ICompetitionClient>>();
            var clientsMock = new Mock<IHubClients<ICompetitionClient>>();
            var clientMock = new Mock<ICompetitionClient>();

            var competitionId = Guid.NewGuid();
            var groupName = CompetitionHub.GetGroupName(competitionId);

            competitionHubMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(groupName)).Returns(clientMock.Object);

            var service = new SignalRRealTimeNotificationService(scoreboardHubMock.Object, competitionHubMock.Object);
            var phase = new CompetitionPhaseNotificationDto
            {
                CompetitionId = competitionId,
                PhaseName = "Judging",
                Status = "Started",
                StartTime = DateTimeOffset.UtcNow,
                Message = "Evaluation stage has begun"
            };

            // Act
            await service.BroadcastPhaseChangeAsync(competitionId, phase);

            // Assert
            clientMock.Verify(c => c.ReceivePhaseChange(phase), Times.Once);
        }
    }
}

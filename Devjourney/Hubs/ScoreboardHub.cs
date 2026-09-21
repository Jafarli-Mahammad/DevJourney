using System;
using System.Threading.Tasks;
using Devjourney.Hubs.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Devjourney.Hubs
{
    public class ScoreboardHub : Hub<IScoreboardClient>
    {
        public static string GetGroupName(Guid competitionId) => $"competition-scoreboard-{competitionId}";

        public async Task JoinScoreboard(Guid competitionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(competitionId));
        }

        public async Task LeaveScoreboard(Guid competitionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(competitionId));
        }
    }
}

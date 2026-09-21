using System;
using System.Threading.Tasks;
using Devjourney.Hubs.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Devjourney.Hubs
{
    public class CompetitionHub : Hub<ICompetitionClient>
    {
        public static string GetGroupName(Guid competitionId) => $"competition-{competitionId}";

        public async Task JoinCompetition(Guid competitionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(competitionId));
        }

        public async Task LeaveCompetition(Guid competitionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(competitionId));
        }
    }
}

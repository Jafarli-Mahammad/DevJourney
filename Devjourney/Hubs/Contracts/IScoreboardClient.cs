using System.Threading.Tasks;
using Application.Common.Models.RealTime;

namespace Devjourney.Hubs.Contracts
{
    public interface IScoreboardClient
    {
        Task ReceiveScoreboardDelta(ScoreboardDeltaDto delta);
        Task ReceiveLeaderboardRefresh(object payload);
    }
}

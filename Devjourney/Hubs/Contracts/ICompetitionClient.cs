using System.Threading.Tasks;
using Application.Common.Models.RealTime;

namespace Devjourney.Hubs.Contracts
{
    public interface ICompetitionClient
    {
        Task ReceivePhaseChange(CompetitionPhaseNotificationDto phase);
        Task ReceiveAnnouncement(CompetitionAnnouncementDto announcement);
        Task ReceiveTimerSync(TimerSyncDto timer);
    }
}

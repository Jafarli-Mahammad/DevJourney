using Application.Repositories.Competitions;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Competition;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Repositories.Competitions
{
    public class CompetitionParticipantRepository : AsyncRepository<CompetitionParticipant>, ICompetitionParticipantRepository
    {
        public CompetitionParticipantRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

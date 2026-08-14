using Application.Repositories.Competitions;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Competition;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Repositories.Competitions
{
    public class CompetitionRepository : AsyncRepository<Competition>, ICompetitionRepository
    {
        public CompetitionRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

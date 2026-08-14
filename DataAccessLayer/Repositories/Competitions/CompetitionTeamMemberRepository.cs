using Application.Repositories.Competitions;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Competition;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Repositories.Competitions
{
    public class CompetitionTeamMemberRepository : AsyncRepository<CompetitionTeamMember>, ICompetitionTeamMemberRepository
    {
        public CompetitionTeamMemberRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

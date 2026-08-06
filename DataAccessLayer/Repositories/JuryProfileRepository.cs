using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Jury;

namespace DataAccessLayer.Repositories
{
    public class JuryProfileRepository : AsyncRepository<JuryProfile>, IJuryProfileRepository
    {
        public JuryProfileRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}

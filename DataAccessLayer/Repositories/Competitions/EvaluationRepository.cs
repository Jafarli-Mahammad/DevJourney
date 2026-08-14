using Application.Repositories.Competitions;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Competition;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Repositories.Competitions
{
    public class EvaluationRepository : AsyncRepository<Evaluation>, IEvaluationRepository
    {
        public EvaluationRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

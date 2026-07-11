using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.University;

namespace DataAccessLayer.Repositories
{
    public class UniversityProfileRepository : AsyncRepository<UniversityProfile>, IUniversityProfileRepository
    {
        public UniversityProfileRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}
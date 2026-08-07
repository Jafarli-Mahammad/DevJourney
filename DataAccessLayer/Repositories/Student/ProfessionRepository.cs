using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;

namespace DataAccessLayer.Repositories
{
    public class ProfessionRepository : AsyncRepository<Profession>, IProfessionRepository
    {
        public ProfessionRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;

namespace DataAccessLayer.Repositories
{
    public class SkillRepository : AsyncRepository<Skill>, ISkillRepository
    {
        public SkillRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}
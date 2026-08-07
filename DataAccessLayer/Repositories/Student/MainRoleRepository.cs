using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;

namespace DataAccessLayer.Repositories
{
    public class MainRoleRepository : AsyncRepository<MainRole>, IMainRoleRepository
    {
        public MainRoleRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

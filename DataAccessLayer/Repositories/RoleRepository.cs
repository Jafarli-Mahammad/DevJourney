using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities;

namespace DataAccessLayer.Repositories
{
    public class RoleRepository : AsyncRepository<Role>, IRoleRepository
    {
        public RoleRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

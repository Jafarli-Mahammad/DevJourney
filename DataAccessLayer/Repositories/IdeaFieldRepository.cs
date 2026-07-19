using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities;

namespace DataAccessLayer.Repositories
{
    public class IdeaFieldRepository : AsyncRepository<IdeaField>, IIdeaFieldRepository
    {
        public IdeaFieldRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

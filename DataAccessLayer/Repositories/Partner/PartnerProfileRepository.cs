using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Partner;

namespace DataAccessLayer.Repositories
{
    public class PartnerProfileRepository : AsyncRepository<PartnerProfile>, IPartnerProfileRepository
    {
        public PartnerProfileRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}
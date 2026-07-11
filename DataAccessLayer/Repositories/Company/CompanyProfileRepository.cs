using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Company;

namespace DataAccessLayer.Repositories
{
    public class CompanyProfileRepository : AsyncRepository<CompanyProfile>, ICompanyProfileRepository
    {
        public CompanyProfileRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}
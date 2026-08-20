using Application.Repositories.Company;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Company;

namespace DataAccessLayer.Repositories.Company
{
    public class CompanyInvitationRepository : AsyncRepository<CompanyInvitation>, ICompanyInvitationRepository
    {
        public CompanyInvitationRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}

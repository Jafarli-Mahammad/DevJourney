using Application.Repositories.Core;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Core;

namespace DataAccessLayer.Repositories.Core
{
    public class CertificateRepository : AsyncRepository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(DataContext context) : base(context)
        {
        }
    }
}

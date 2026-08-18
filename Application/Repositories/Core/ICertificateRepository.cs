using Domain.Models.Entities.Core;
using System.Threading.Tasks;

namespace Application.Repositories.Core
{
    public interface ICertificateRepository : IAsyncRepository<Certificate>
    {
    }
}

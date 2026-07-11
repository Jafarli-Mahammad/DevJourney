using Application.Repositories;
using DataAccessLayer.DataContexts;
using Domain.Models.Entities.Student;

namespace DataAccessLayer.Repositories
{
    public class LanguageRepository : AsyncRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(DataContext dataContext)
            : base(dataContext)
        {
        }
    }
}
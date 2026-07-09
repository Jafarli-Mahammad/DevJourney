using System.Linq.Expressions;

namespace Application.Repositories
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Expression<Func<T, bool>>? expression = null,
                          Func<IQueryable<T>, IQueryable<T>>? include = null,
                          CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> EditAsync(T entity);
        void Remove(T entity);
    }
}
using Application.Exceptions;
using Application.Repositories;
using DataAccessLayer.DataContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories
{
    public class AsyncRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly DataContext DataContext;

        public AsyncRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null,
                                                CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DataContext.Set<T>().AsNoTracking();
            if (expression is not null)
                query = query.Where(expression);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = DataContext.Set<T>().AsNoTracking();

            if (expression is not null)
            {
                query = query.Where(expression);
            }

            if (include is not null)
            {
                query = include(query);
            }

            var entity = await query.FirstOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name, expression?.ToString() ?? "entity");
            }

            return entity;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DataContext.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task<T> EditAsync(T entity)
        {
            DataContext.Set<T>().Update(entity);
            await Task.CompletedTask;
            return entity;
        }

        public void Remove(T entity)
        {
            DataContext.Set<T>().Remove(entity);
        }
    }
}
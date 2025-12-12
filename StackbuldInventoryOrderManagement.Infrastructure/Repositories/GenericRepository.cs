using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;
using StackbuldInventoryOrderManagement.Common.Utilities;
using StackbuldInventoryOrderManagement.Persistence.Context;
using System.Linq.Expressions;

namespace StackbuldInventoryOrderManagement.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<GenericRepository<T>> _logger;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _logger = _dbContext.GetService<ILogger<GenericRepository<T>>>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public IQueryable<T> FindByCondition(
            Expression<Func<T, bool>> expression,
            bool trackChanges
        )
        {
            return !trackChanges
                ? _dbSet.Where(expression).AsNoTracking()
                : _dbSet.Where(expression);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression ?? (x => true));
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public async Task<IEnumerable<T>> FindAndIncludeAsync(
            Expression<Func<T, bool>> expression,
            params string[] includeProperties
        )
        {
            IQueryable<T> query = _dbSet;

            if (expression != null)
                query = query.Where(expression);

            foreach (var include in includeProperties.Where(p => !p.Contains('.')))
            {
                query = query.Include(include);
            }

            foreach (var includePath in includeProperties.Where(p => p.Contains('.')))
            {
                query = query.Include(includePath);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params string[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = includeProperties.Aggregate(
                query,
                (current, includeProperty) => current.Include(includeProperty)
            );
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetByIdAsync2(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.CountAsync(expression);
        }

        public IQueryable<T> IncludeWithThenInclude(
            Expression<Func<T, object>> includeProperty,
            params Expression<Func<object, object>>[] thenIncludeProperties
        )
        {
            IIncludableQueryable<T, object> query = _dbSet.Include(includeProperty);

            foreach (var thenIncludeProperty in thenIncludeProperties)
            {
                query = query.ThenInclude(thenIncludeProperty);
            }

            return (IQueryable<T>)query;
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<bool> SaveAsync()
        {
            if (await _dbContext.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }

        public async Task<PagedResult<T>> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params string[] includeProperties
        )
        {
            try
            {
                IQueryable<T> query = _dbSet;

                if (filter != null)
                    query = query.Where(filter);

                query = includeProperties.Aggregate(
                    query,
                    (current, includeProperty) => current.Include(includeProperty)
                );

                if (orderBy != null)
                    query = orderBy(query);

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                return new PagedResult<T>
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Items = items,
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paged data");
                throw;
            }
        }
    }
}

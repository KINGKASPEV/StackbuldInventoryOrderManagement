using StackbuldInventoryOrderManagement.Common.Utilities;
using System.Linq.Expressions;

namespace StackbuldInventoryOrderManagement.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T>
        where T : class
    {
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
        Task<T> FindAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindAndIncludeAsync(
            Expression<Func<T, bool>> expression,
            params string[] includeProperties
        );
        Task<IEnumerable<T>> GetAllAsync(params string[] includeProperties);

        Task<T> GetByIdAsync(string id);
        Task<T> GetByIdAsync2(Guid id);

        //Task<T> GetByIdAsync<TKey>(TKey id);
        Task RemoveOutLetAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        IQueryable<T> IncludeWithThenInclude(
            Expression<Func<T, object>> includeProperty,
            params Expression<Func<object, object>>[] thenIncludeProperties
        );
        Task<T> GetWithPermissionsAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
        Task<int> CountAsync();
        Task<bool> SaveAsync();
        Task<PagedResult<T>> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params string[] includeProperties
        );
    }
}
